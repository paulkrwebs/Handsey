using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey
{
    public interface IHandlerFactory
    {
        IList<HandlerInfo> Create(Type handlerBaseType, Type[] types);

        HandlerInfo Create(Type handlerBaseType, Type type);
    }
}