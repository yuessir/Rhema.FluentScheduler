using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using FluentScheduler;
using Serilog;

namespace Rhema.FluentScheduler
{
    public interface IScheduleJobEngine : IExecutionContext
    {
        IScheduleJobEngine Run();
        IScheduleJobEngine Stop();

        /// <summary>
        /// 手動執行排成 Manual Job >>need to RUN();
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

        public ScheduleJobEngine(ILogger logger)
        {
            _logger = logger;
        }


        public IScheduleJobEngine AddJob(Action job, Action<Schedule> schedule)
        {
            AddJob(schedule, new Schedule(job));

            return this;
        }
        private string _excuteContextName;

        private void SetExcuteContextName(Schedule schedule)
        {
            _excuteContextName = schedule.Name ?? Guid.NewGuid().ToString();
        }
        private void AddJob(Action<Schedule> jobSchedule, Schedule schedule)
        {
            if (schedule.Name == null)
            {

                schedule.WithName(Guid.NewGuid().ToString());
            }
            var ty = typeof(JobManager);
            jobSchedule(schedule);
            if (schedule.NextRun == default(DateTime))
            {
                var prop = schedule.GetType().GetProperty("DelayRunFor", BindingFlags.Instance |
                                                                         BindingFlags.NonPublic);
                var propVal = prop.GetValue(schedule);
                if ((TimeSpan)propVal==TimeSpan.Zero)
                {
                    schedule.ToRunNow().DelayFor(1).Milliseconds();
                }
            }

            var n = new Schedule[] { schedule };
            var fun = ty.GetMethod("CalculateNextRun", BindingFlags.Static | BindingFlags.NonPublic);
            var list = fun.Invoke(null, new object[] { n });
            var list2 = ((IEnumerable<Schedule>)list).ToList();
            if (schedule.NextRun < DateTime.Now)
            {
                //DO NOT RUN JOB
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
            _map.Remove(name);
            JobManager.RemoveJob(name);
            return this;
        }
        public IScheduleJobEngine Stop()
        {
            JobManager.StopAndBlock();
            return this;
        }



        private static event Action<JobEndInfo> JobEndHandler;
        private static Dictionary<string, object> _map = new Dictionary<string, object>();

        public IExecutionContext JobEnd<T>(JobEndData<T> endArgs, Func<JobEndData<T>, object> func = null)
        {
            if (func == null) return this;
            _endArgs = endArgs;

            if (!_map.ContainsKey(_excuteContextName))
            {
                _map.Add(_excuteContextName, func);
            }


            if (JobEndHandler == null)
            {
                JobEndHandler += info =>
                {
                    if (_map.ContainsKey(info.Name))
                    {
                        var func2 = (Func<JobEndData<T>, object>)_map[info.Name];
                        endArgs.Info = info;
                        func2(endArgs);
                        if (_logger != null)
                        {
                            _logger.Warning(info.Name + " JobEndTime : " + info.StartTime);
                        }
                    }
                };

                JobManager.JobEnd += JobEndHandler;
            }

            return this;
        }




    }
}