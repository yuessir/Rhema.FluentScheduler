using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentScheduler;

namespace Rhema.FluentScheduler
{
    public class RhemaJobEndInfo : JobEndInfo
    {
        public string Name { get; set; }
        public string JobName { get; }

        public DateTime StartTime { get; }

        public TimeSpan Duration { get; }

        public DateTime? NextRun { get; }
        public RhemaJobEndInfo(JobEndInfo innerInfo)
        {
            if (innerInfo != null)
            {
                NextRun = innerInfo.NextRun;
                StartTime = innerInfo.StartTime;
                JobName = innerInfo.Name;
                Duration = innerInfo.Duration;
            }
        }
        public RhemaJobEndInfo(string innerInfo)
        {
            Name = innerInfo;
        }
    }
    public class RhemaJobExceptionInfo : JobExceptionInfo { }
}
