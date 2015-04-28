using Handsey.Handlers;
using Handsey.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey
{
    /// <summary>
    /// Thread safe container for handlers
    /// </summary>
    public class ApplicationHandlers : IApplicationHandlers
    {
        private readonly ConcurrentQueue<HandlerInfo> _handles;

        public ApplicationHandlers(IList<HandlerInfo> handles)
        {
            PerformCheck.IsNull(handles).Throw<ArgumentNullException>(() => new ArgumentNullException("Handles parameter cannot be null"));

            _handles = new ConcurrentQueue<HandlerInfo>(handles);
        }

        public virtual IEnumerable<HandlerInfo> Find(HandlerInfo toSearchFor, IHandlerSearch search)
        {
            PerformCheck.IsNull(search, toSearchFor).Throw<ArgumentNullException>(() => new ArgumentNullException("Search parameter cannot be null"));
            PerformCheck.IsNull(() => toSearchFor.Type).Throw<ArgumentNullException>(() => new ArgumentNullException("Search parameter cannot be null"));

            // Double dispatch
            // The search should return a new list so is thread safe
            return search.Execute(toSearchFor, _handles);
        }
    }
}