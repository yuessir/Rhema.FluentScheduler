using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rhema.FluentScheduler
{
    public interface IExecutionContext
    {
        IExecutionContext JobStart(object startArgs, Func<RhemaJobStartInfo, object, object> func = null);
        IExecutionContext JobStart<T>(T startArgs, Action<T> func = null);
        IExecutionContext JobStart(Action func = null);
        IExecutionContext JobEnd(object endArgs, Func<RhemaJobEndInfo, object, object> func = null);
        IExecutionContext JobEnd<T>(T endArgs, Action<RhemaJobEndInfo, object> func = null);
        IExecutionContext JobException(Func<RhemaJobExceptionInfo, object> func = null);

    }

}
