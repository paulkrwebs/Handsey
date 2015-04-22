using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey
{
    public class ApplicationHandlersFactory : IApplicationHandlersFactory
    {
        public IApplicationHandlers Create(IList<HandlerInfo> handlers)
        {
            throw new NotImplementedException();
        }
    }
}