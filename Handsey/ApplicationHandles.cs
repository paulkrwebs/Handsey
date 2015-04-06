using Handsey.Handlers;
using Handsey.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey
{
    public class ApplicationHandles
    {
        private readonly IList<TypeInfo> _handles;
        private readonly IHandlerFactory _handlerFactory;

        public ApplicationHandles(IList<TypeInfo> handles)
        {
            NullCheck.ThrowIfNull<ArgumentNullException>(handles, () => new ArgumentNullException("Handles parameter cannot be null"));

            _handles = handles;
        }

        public virtual bool PreviouslyAttemptedToFind(TypeInfo toSearchFor)
        {
            throw new NotFiniteNumberException("TODO Look up from thread safe list");
        }

        public virtual IEnumerable<TypeInfo> Find(TypeInfo toSearchFor, IHandlerSearch search)
        {
            NullCheck.ThrowIfNull<ArgumentNullException>(search, () => new ArgumentNullException("Search parameter cannot be null"));

            // TODO Add toSearchFor to thread safe collection so we can remember we have already searched for this type

            // this is going to be a double dispatch method :)
            return search.Execute(toSearchFor, _handles);
        }
    }
}