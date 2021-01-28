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

        public static void StopAll<T>(
            this JobInfo<T> info)
        {
            JobManager.StopAndBlock();
        }

        public static void Stop<T>(
            this JobInfo<T> info, string jobName)

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
