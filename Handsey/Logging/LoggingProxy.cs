using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey.Logging
{
    public class LoggingProxy : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();

            invocation.Proceed();

            stopwatch.Stop();

            Debug.WriteLine("Method: {0}, Returned Value: {1}, Took: {2} ms / {3} ticks"
                , invocation.MethodInvocationTarget.Name
                , invocation.ReturnValue
                , stopwatch.ElapsedMilliseconds
                , stopwatch.ElapsedTicks);
        }
    }
}