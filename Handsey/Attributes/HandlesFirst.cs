using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey.Attributes
{
    /// <summary>
    //// Always the first handler to execute. If there are multiple Firsts then the order of First handlers cannot be predicted
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class HandlesFirst : HandlerAttribute
    {
        public HandlesFirst()
        {
            ExecutionOrder = ExecutionOrder.First;
        }
    }
}