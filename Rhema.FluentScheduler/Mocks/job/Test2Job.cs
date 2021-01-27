using System;
using System.Threading;
using Serilog;

namespace Rhema.FluentScheduler
{
    public class Test2Job : RhemaTask
    {
        public ILogger _logger;
        public IBook _book;
        public Test2Job(ILogger logger, IBook book) : base(logger)
        {
            _logger = logger;
            _book = book;
            OnBeforeExcute = () =>
            {
                _logger.Warning(" step1... OnBeforeExcute");
                Console.WriteLine("step1.Test2Job.. OnBeforeExcute");
            };

            OnAfterExcute = () =>
            {
                _logger.Fatal("step3..Test2Job...  OnAfterExcute");
                Console.WriteLine("step3..Test2Job...  OnAfterExcute");
            };
          

        }

        protected override void OnExecuting()
        {
          
            Console.WriteLine(_book.GetBookId());
            Console.WriteLine("Test2Job OKK");
            Thread.Sleep(3000);

           
        }
    }

    public interface IBook
    {
          int GetBookId();
    }
    public class Book: IBook
    {
       public int GetBookId()
        {
            return 0;
        }
    }
}
