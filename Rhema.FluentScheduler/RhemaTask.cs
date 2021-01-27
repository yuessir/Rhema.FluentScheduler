using System;
using Serilog;

namespace Rhema.FluentScheduler
{
    public abstract class RhemaTask : SchduleJobBase
    {
        public ILogger _logger;
        public RhemaTask(ILogger logger)
        {
            _logger = logger;
            Logger = _logger;
        }


        protected Action OnBeforeExcute = () => { };
   
        protected Action OnAfterExcute = () => { };

        public override void OnExecute()
        {
            OnBeforeExcute();
            OnExecuting();
            OnAfterExcute();
        }

        protected abstract void OnExecuting();
    }
    public class ExecutionHandle { }

   
}
