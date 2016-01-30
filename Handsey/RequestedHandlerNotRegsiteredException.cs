using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey
{
    public class RequestedHandlerNotRegsiteredException : Exception
    {
        public RequestedHandlerNotRegsiteredException(string message)
            : base(message)
        { }
    }
}