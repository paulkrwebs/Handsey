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
            NullCheck.ThowIfNull<ArgumentNullException>(handles, () => new ArgumentNullException("Handles parameter cannot be null"));

            _handles = handles;
        }

        public IEnumerable<TypeInfo> Find(TypeInfo toSearchFor, IHandlerSearch search)
        {
            NullCheck.ThowIfNull<ArgumentNullException>(search, () => new ArgumentNullException("Search parameter cannot be null"));

            // this is going to be a double dispatch method :)
            return search.Execute(toSearchFor, _handles);
        }
    }
}