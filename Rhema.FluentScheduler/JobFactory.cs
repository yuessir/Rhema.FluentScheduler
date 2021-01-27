using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using FluentScheduler;
using Serilog;

namespace Rhema.FluentScheduler
{
    public class RhemaJobFactory : IJobFactory
    {
        private static IContainer _container;
        public RhemaJobFactory() 
        {
            if (_container!=null)
            {
               return;
            }
       
            _container = IoC.Container;
        } 
        public IJob GetJobInstance<T>() where T : IJob
        {
            return _container.Resolve<T>();
        }
    }


    public class IoC
    {
        private static IContainer _container;
        public static IContainer Container { get; set; }
        private static ILifetimeScope _lifetimeScope;
        public static ILifetimeScope GetLifetimeScope()
        {
            return _lifetimeScope ?? (_lifetimeScope = _container.BeginLifetimeScope());
        }
        public static ILifetimeScope GetLifetimeScope(object tag)
        {
            return _lifetimeScope ?? (_lifetimeScope = _container.BeginLifetimeScope(tag));
        }
        public static IContainer InitializeAutoFacContainer()
        {
            if (_lifetimeScope != null)
            {
                _lifetimeScope.Dispose();
                _lifetimeScope = null;
            }
     
            ILogger innerlogger = new LoggerConfiguration().CreateLogger();
            var tt = Assemblies.GetSchedulerServiceAssemblies.Where(x => x.GetName().Name == "Rhema.FluentScheduler");
            var builder = new ContainerBuilder();
            builder.RegisterInstance(innerlogger);
            var assemblies = Assemblies.GetSchedulerServiceAssemblies.ToArray();
            builder.RegisterAssemblyTypes(assemblies).AsImplementedInterfaces();
         
            builder.RegisterAssemblyTypes(assemblies.Where(x => x.GetName().Name == "Rhema.FluentScheduler").ToArray())
                .Where(x => x.Name.EndsWith("Job"))
                .AsSelf();
            builder.RegisterType<ScheduleJobEngine>().As<IScheduleJobEngine>();
            builder.RegisterType<ContextEngine>().As<IContextEngine>();
            _container = builder.Build();
            Container = _container;
            return _container;
        }
      
        public static class Assemblies
        {
            private static readonly Assembly SchedulerService = Assembly.GetAssembly(typeof(IoC));

            public static IEnumerable<Assembly> GetSchedulerServiceAssemblies
            {
                get
                {
                    yield return SchedulerService;
                  
                }
            }
        }
    }
}
