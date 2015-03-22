using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey
{
    public interface IHandlerFactory
    {
        IList<ClassInfo> Create(Type handlerBaseType, Type[] types);

        ClassInfo Create(Type handlerBaseType, Type type);
    }
}
