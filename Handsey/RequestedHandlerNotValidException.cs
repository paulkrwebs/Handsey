﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey
{
    public class RequestedHandlerNotValidException : Exception
    {
        public RequestedHandlerNotValidException(string message)
            : base(message)
        { }
    }
}