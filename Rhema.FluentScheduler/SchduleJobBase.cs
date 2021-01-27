using FluentScheduler;
using Serilog;

namespace Rhema.FluentScheduler
{
    public abstract class SchduleJobBase : IRhemaTask
    {
        public SchduleJobBase() => Disposed = false;
        public static bool Disposed { get; private set; }
        public void Dispose() => Disposed = true;
        private readonly object _lock = new object();
        private bool _stop;
        public abstract void OnExecute();
        public virtual void Execute()
        {
            lock (_lock)
            {
                if (_stop)
                {
                    return;
                }

                OnExecute();
            }
        }


        public ILogger Logger { get; set; }
    }
    public interface IRhemaTask : IJob
    {
        ILogger Logger { get; set; }
        void Execute();
    }

}
