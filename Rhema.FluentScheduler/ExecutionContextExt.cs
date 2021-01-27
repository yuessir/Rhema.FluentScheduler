using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac.Core;
using FluentScheduler;

namespace Rhema.FluentScheduler
{
    public static class ExecutionContextExt
    {

        public static void StopAll(
            this RhemaJobEndInfo info)

        {
            foreach (var schedule in JobManager.AllSchedules)
            {
                if (schedule.Name == info.Name)
                {
                    schedule.Disable();
                    JobManager.RemoveJob(info.Name);
                }
            }

        }

        public static void Stop(
            this RhemaJobEndInfo info, string jobName)

        {
            var schedule = JobManager.GetSchedule(jobName);
            if (schedule != null)
            {
                schedule.Disable();
                JobManager.RemoveJob(jobName);
            }

        }
        public static void AddRhemaJobFactory(
            this Container container)

        {
            JobManager.JobFactory = new RhemaJobFactory();
        }
    }
}
