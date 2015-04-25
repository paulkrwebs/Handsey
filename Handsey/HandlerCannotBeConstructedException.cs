using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey
{
    public class HandlerCannotBeConstructedException : Exception
    {
        public HandlerCannotBeConstructedException()
        { }

        public HandlerCannotBeConstructedException(string message)
            : base(message)
        { }
    }
}