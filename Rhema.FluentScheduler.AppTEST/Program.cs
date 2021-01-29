using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using FluentScheduler;

namespace Rhema.FluentScheduler.AppTEST
{
    class Program
    {
        static void Main(string[] args)
        {
            var _container = IoC.InitializeAutoFacContainer();
            JobManager.JobFactory = new RhemaJobFactory();

            var eng = _container.Resolve<IScheduleJobEngine>();
      
            eng.AddJob(() =>
                {
                    Console.WriteLine("DelayFor message.");
                },
                s => s.WithName("TestJob7").ToRunNow().DelayFor(10).Seconds());
            eng.AddJob(() =>
                {
                    Console.WriteLine("ToRunEvery message."+DateTime.Now.ToString());
                },
                s => s.WithName("TestJob8").ToRunEvery(3).Seconds());
            eng.AddJob(() =>
                {
                    Console.WriteLine("Do something after the job.");
                },
                s => s.WithName("TestJob8").ToRunEvery(5).Seconds()).JobEnd(new JobEndData<DateTime>(){Data = DateTime.Now},GetMessage);
            eng.Run();
            Console.ReadKey();
            eng.Stop();
        }

        private static object GetMessage(JobEndData<DateTime> arg)
        {
            Console.WriteLine(arg.Data.ToString());
            return null;
        }
    }
}
