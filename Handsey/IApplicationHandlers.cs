using System;
using System.Collections.Generic;

namespace Handsey
{
    public interface IApplicationHandlers
    {
        void ClearPreviousFindAttemptsCache();

        IEnumerable<HandlerInfo> Find(HandlerInfo toSearchFor, IHandlerSearch search);

        bool PreviouslyAttemptedToFind(HandlerInfo toSearchFor);
    }
}