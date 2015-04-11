using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
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