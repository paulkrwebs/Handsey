using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey.Attributes
{
    /// <summary>
    //// Always the last handler to execute. If there are multiple Lasts then the order of Last handlers cannot be predicted
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class HandlesLast : HandlerAttribute
    {
        public HandlesLast()
        {
            ExecutionOrder = ExecutionOrder.Last;
        }
    }
}