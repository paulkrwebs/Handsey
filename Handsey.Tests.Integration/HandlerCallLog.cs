using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey.Tests.Integration
{
    public class HandlerCallLog : IHandlerCallLog
    {
        public List<Type> Log { get; set; }

        public HandlerCallLog()
        {
            Log = new List<Type>();
        }
    }
}