using System;
using System.Collections.Generic;

namespace Handsey
{
    public interface IApplicationHandlers
    {
        IEnumerable<HandlerInfo> Find(HandlerInfo toSearchFor, IHandlerSearch search);
    }
}