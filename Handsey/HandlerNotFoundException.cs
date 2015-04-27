using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey
{
    public class HandlerNotFoundException : Exception
    {
        public HandlerNotFoundException(string message)
            : base(message)
        { }
    }
}