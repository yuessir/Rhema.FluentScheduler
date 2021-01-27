using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentScheduler;

namespace Rhema.FluentScheduler
{
   
    public class RhemaJobStartInfo : JobStartInfo
    {
        public string Name { get; set; }

        public DateTime StartTime { get; set; }
        public RhemaJobStartInfo(JobStartInfo innerInfo)
        {
            StartTime = innerInfo.StartTime;
            Name = innerInfo.Name;
        }
    }
}
