using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey.Attributes
{
    /// <summary>
    /// Executed after the handlers specified in the type argument have been executed.
    /// This attribute can be specified more than once on the same class
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class HandlesAfter : HandlerAttribute
    {
        public Type Type { get; private set; }

        public HandlesAfter(Type type)
        {
            Type = type;
            ExecutionOrder = ExecutionOrder.InSequence;
        }
    }
}