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
    public class ApplicationHandles
    {
        private readonly IList<TypeInfo> _handles;
        private readonly ConcurrentDictionary<Type, bool> _previousFindAttemptsCache;

        public ApplicationHandles(IList<TypeInfo> handles)
        {
            PerformCheck.IsNull(handles).Throw<ArgumentNullException>(() => new ArgumentNullException("Handles parameter cannot be null"));

            _handles = handles;
            _previousFindAttemptsCache = new ConcurrentDictionary<Type, bool>();
        }

        public virtual void ClearPreviousFindAttemptsCache()
        {
            _previousFindAttemptsCache.Clear();
        }

        public virtual bool PreviouslyAttemptedToFind(TypeInfo toSearchFor)
        {
            PerformCheck.IsNull(toSearchFor).Throw<ArgumentNullException>(() => new ArgumentNullException("Search parameter cannot be null"));
            PerformCheck.IsNull(() => toSearchFor.Type).Throw<ArgumentNullException>(() => new ArgumentNullException("Search parameter cannot be null"));

            bool foo;
            return _previousFindAttemptsCache.TryGetValue(toSearchFor.Type, out foo);
        }

        public virtual IEnumerable<TypeInfo> Find(TypeInfo toSearchFor, IHandlerSearch search)
        {
            PerformCheck.IsNull(search, toSearchFor).Throw<ArgumentNullException>(() => new ArgumentNullException("Search parameter cannot be null"));
            PerformCheck.IsNull(() => toSearchFor.Type).Throw<ArgumentNullException>(() => new ArgumentNullException("Search parameter cannot be null"));

            // Add toSearchFor to thread safe collection so we can remember we have already searched for this type
            _previousFindAttemptsCache.TryAdd(toSearchFor.Type, true);

            // this is going to be a double dispatch method :)
            return search.Execute(toSearchFor, _handles);
        }
    }
}