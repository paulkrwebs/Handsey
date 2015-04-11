using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class HandlesFirst : HandlerAttribute
    {
        public HandlesFirst()
        {
            ExecutionOrder = ExecutionOrder.First;
        }
    }
}