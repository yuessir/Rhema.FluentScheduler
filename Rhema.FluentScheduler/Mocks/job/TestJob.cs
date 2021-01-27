using System;
using System.Threading;
using Serilog;

namespace Rhema.FluentScheduler
{
    public class TestJob : RhemaTask
    {
        public ILogger _logger;
        public TestJob(ILogger logger) : base(logger)
        {
            _logger = logger;
            OnBeforeExcute = () =>
            {
                _logger.Warning(" step1... OnBeforeExcute");
                Console.WriteLine("step1... OnBeforeExcute");
            };

            OnAfterExcute = () =>
            {
                _logger.Fatal("step3.....  OnAfterExcute");
                Console.WriteLine("step3.....  OnAfterExcute");
            };
            
        }

        protected override void OnExecuting()
        {

            Console.WriteLine("error occurred");
         
         //   throw new InvalidOperationException("Cannot read temperature before initializing.");


        }
    }
}
