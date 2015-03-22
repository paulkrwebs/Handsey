using Handsey.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey
{
    public class ApplicationHandles
    {
        private readonly IList<ClassInfo> _classes;

        public ApplicationHandles(IList<ClassInfo> classes)
        {
            _classes = classes;
        }

        public void Find<THandler>(IHandleSearch search)
            where THandler : IHandles
        {
            // this is going to be a double dispatch method :)
            // search.Perform<THandler>(_classes);
            throw new NotImplementedException("TODO");
        }
    }
}