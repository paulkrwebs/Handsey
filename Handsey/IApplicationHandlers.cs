using System;

namespace Handsey
{
    public interface IApplicationHandlers
    {
        void ClearPreviousFindAttemptsCache();

        System.Collections.Generic.IEnumerable<HandlerInfo> Find(HandlerInfo toSearchFor, IHandlerSearch search);

        bool PreviouslyAttemptedToFind(HandlerInfo toSearchFor);
    }
}