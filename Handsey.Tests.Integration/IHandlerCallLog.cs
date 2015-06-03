using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Handsey.Tests.Integration
{
    public interface IHandlerCallLog
    {
        List<Type> Log { get; set; }
    }
}