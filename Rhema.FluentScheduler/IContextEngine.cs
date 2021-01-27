using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;

namespace Rhema.FluentScheduler
{
    public interface IContextEngine
    {
        T Resolve<T>() where T : class;
    }
    public class ContextEngine : IContextEngine
    {
        public T Resolve<T>() where T : class
        {
            return (T)Resolve(typeof(T));
        }
        private object Resolve(Type type)
        {
            return IoC.Container.Resolve(type);
        }

        public static IContextEngine Current
        {
            get
            {
                if (IoC.Container == null) return null;
                var _container = IoC.Container;
                var ctxEngine = _container.Resolve<IContextEngine>();
                return ctxEngine;


            }
        }

    }
}
