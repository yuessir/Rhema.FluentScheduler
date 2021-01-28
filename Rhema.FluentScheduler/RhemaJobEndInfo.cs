using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentScheduler;

namespace Rhema.FluentScheduler
{
    public interface IJobData<T>
    {

        string Name { get; set; }
        T Data { get; set; }
    }

    public abstract class JobInfo<T>
    {
        public T Info { get; set; }


    }
    public class JobEndData<T> : JobInfo<JobEndInfo>, IJobData<T>
    {
        public string Name { get; set; }
        public T Data { get; set; }
    }
    public class JobStartData<T> : JobInfo<JobStartInfo>, IJobData<T>
    {
        public string Name { get; set; }
        public T Data { get; set; }
    }
    public class JobExceptionData<T> : JobInfo<JobExceptionInfo>, IJobData<T>
    {
        public string Name { get; set; }
        public T Data { get; set; }
    }

}
