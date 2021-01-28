using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rhema.FluentScheduler
{
    public interface IExecutionContext
    {
    
        IExecutionContext JobEnd<T>(JobEndData<T> endArgs, Func<JobEndData<T>, object> func = null);

    }

}
