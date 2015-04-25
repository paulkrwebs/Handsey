using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey
{
    public class RequestedHandlerNotValid : Exception
    {
        public RequestedHandlerNotValid()
        { }

        public RequestedHandlerNotValid(string message)
            : base(message)
        { }
    }
}