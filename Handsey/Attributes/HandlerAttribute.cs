using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey.Attributes
{
    public class HandlerAttribute : Attribute
    {
        public ExecutionOrder ExecutionOrder { get; protected set; }
    }
}