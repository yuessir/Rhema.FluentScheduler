using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentScheduler;
using Serilog;

namespace Rhema.FluentScheduler
{
    public interface IScheduleJobEngine : IExecutionContext
    {
        IScheduleJobEngine WithRegistry<T>() where T : Registry, new();
        IScheduleJobEngine WithRegistry<T>(T taskRegistry) where T : Registry;
        IScheduleJobEngine Run();
        IScheduleJobEngine Stop();
        /// <summary>
        /// 自動執行多排成
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="schedules"></param>
        /// <returns></returns>
        IScheduleJobEngine AddJob<T>(params Action<Schedule>[] schedules) where T : IJob;
        /// <summary>
        /// 手動執行排成
        /// </summary>
        /// <param name="job"></param>
        /// <param name="schedule"></param>
        /// <returns></returns>
        IScheduleJobEngine AddJob(Action job, Action<Schedule> schedule);
        IScheduleJobEngine RemoveJob(string name);
    }

    public class ScheduleJobEngine : IScheduleJobEngine
    {

        private ILogger _logger;
        private object _startArgs;
        private object _endArgs;
        private string _excuteContextName;
        public ScheduleJobEngine(ILogger logger)
        {
            _logger = logger;
        }

        public IScheduleJobEngine WithRegistry<T>() where T : Registry, new()
        {
            JobManager.Initialize(new T());
            return this;
        }

        public IScheduleJobEngine WithRegistry<T>(T taskRegistry) where T : Registry
        {
            JobManager.Initialize(taskRegistry);
            return this;
        }

        public IScheduleJobEngine AddJob<T>(params Action<Schedule>[] schedules) where T : IJob
        {
            foreach (var schedule in schedules)
            {
                JobManager.AddJob<T>(schedule);
            }
            return this;
        }
        public IScheduleJobEngine AddJob(Action job, Action<Schedule> schedule)
        {
            AddJob(schedule, new Schedule(job));
            return this;
        }

        private void SetExcuteContextName(Schedule schedule)
        {
            _excuteContextName = schedule.Name ?? Guid.NewGuid().ToString();
        }
        private void AddJob(Action<Schedule> jobSchedule, Schedule schedule)
        {

            jobSchedule(schedule);
            var ty = typeof(JobManager);
            var n = new Schedule[] { schedule };
            var fun = ty.GetMethod("CalculateNextRun", BindingFlags.Static | BindingFlags.NonPublic);
            var list = fun.Invoke(null, new object[] { n });
            var list2 = ((IEnumerable<Schedule>)list).ToList();
            if (schedule.NextRun < DateTime.Now)
            {
                var fun3 = ty.GetMethod("RunJob", BindingFlags.Static | BindingFlags.NonPublic);
                fun3.Invoke(null, new object[] { schedule });
            }
            else
            {
                var fun2 = ty.GetMethod("ScheduleJobs", BindingFlags.Static | BindingFlags.NonPublic);
                fun2.Invoke(null, null);
            }

            SetExcuteContextName(schedule);
        }

        public IScheduleJobEngine Run()
        {
            JobManager.Start();
            return this;
        }
        public IScheduleJobEngine RemoveJob(string name)
        {
            JobManager.RemoveJob(name);
            return this;
        }
        public IScheduleJobEngine Stop()
        {
            JobManager.StopAndBlock();
            return this;
        }



        public IExecutionContext JobStart(object startArgs, Func<RhemaJobStartInfo, object, object> func = null)
        {
            _startArgs = startArgs;
            if (func != null)
            {
                JobManager.JobStart += info =>
                {
                    var RhemaInfo = new RhemaJobStartInfo(info);
                    func(RhemaInfo, _startArgs);
                    if (_logger != null)
                    {
                        _logger.Warning(info.Name + " JobStartTime : " + info.StartTime);
                    }

                };
            }

            return this;
        }

        public IExecutionContext JobStart<T>(T startArgs, Action<T> func = null)
        {
            _startArgs = startArgs;
            if (func != null)
            {

                JobManager.JobStart += info =>
                {

                    func((T)_startArgs);
                    if (_logger != null)
                    {
                        _logger.Warning(info.Name + " JobStartTime : " + info.StartTime);
                    }

                };
            }

            return this;
        }

        public IExecutionContext JobStart(Action func = null)
        {
            if (func != null)
            {
                JobManager.JobStart += info =>
                {

                    func();
                    if (_logger != null)
                    {

                        _logger.Warning(info.Name + " JobStartTime : " + info.StartTime);
                    }
                };
            }

            return this;
        }

        private Dictionary<string, Func<RhemaJobEndInfo, object, object>> _jobfunctionMap = new Dictionary<string, Func<RhemaJobEndInfo, object, object>>();

        public IExecutionContext JobEnd(object endArgs, Func<RhemaJobEndInfo, object, object> func = null)
        {
            if (func == null) return this;

            _endArgs = endArgs;
            if (!_jobfunctionMap.ContainsKey(_excuteContextName))
            {
                _jobfunctionMap.Add(_excuteContextName, func);
            }

            JobEnd(func);

            return this;
        }

        private event Action<JobEndInfo> JobEndHandler;
        private void JobEnd(Func<RhemaJobEndInfo, object, object> func)
        {

            if (JobEndHandler == null)
            {
                JobEndHandler += info =>
                {
                    var RhemaInfo = new RhemaJobEndInfo(info);
                    if (_jobfunctionMap.ContainsKey(RhemaInfo.JobName))
                    {
                        var func2 = _jobfunctionMap[RhemaInfo.JobName];
                        func2(RhemaInfo, _endArgs);
                        if (_logger != null)
                        {
                            _logger.Warning(info.Name + " JobEndTime : " + info.StartTime);
                        }
                    }
                };

                JobManager.JobEnd += JobEndHandler;
            }

        }
        private Dictionary<string, Action<RhemaJobEndInfo, object>> _jobactionMap = new Dictionary<string, Action<RhemaJobEndInfo, object>>();
        public IExecutionContext JobEnd<T>(T endArgs, Action<RhemaJobEndInfo, object> func = null)
        {
            if (func == null) return this;
            _endArgs = endArgs;
            if (_excuteContextName == null)
            {
                _excuteContextName = Guid.NewGuid().ToString();
            }
            foreach (var schedule in JobManager.AllSchedules)
            {
                if (!_jobactionMap.ContainsKey(schedule.Name))
                {
                    _jobactionMap.Add(schedule.Name, func);
                }
            }


            JobEnd(func);


            return this;
        }
        private event Action<JobEndInfo> JobEndActionHandler;
        private void JobEnd(Action<RhemaJobEndInfo, object> func)
        {

            if (JobEndActionHandler == null)
            {
                JobEndActionHandler += info =>
                {
                    var RhemaInfo = new RhemaJobEndInfo(info);
                    if (_jobactionMap.ContainsKey(RhemaInfo.JobName))
                    {
                        var func2 = _jobactionMap[RhemaInfo.JobName];
                        func2(RhemaInfo, _endArgs);
                        if (_logger != null)
                        {
                            _logger.Warning(info.Name + " JobEndTime : " + info.StartTime);
                        }
                    }
                };

                JobManager.JobEnd += JobEndActionHandler;
            }

        }

        /// <summary>
        /// todo exception map
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public IExecutionContext JobException(Func<RhemaJobExceptionInfo, object> func = null)
        {

            if (func != null)
            {

                JobManager.JobException += info =>
                {

                    func(info as RhemaJobExceptionInfo);
                    if (_logger != null)
                    {
                        _logger.Warning(info.Name + " JobErrorMessage : " + info.Exception.Message);
                    }

                };
            }

            return this;
        }


    }
}