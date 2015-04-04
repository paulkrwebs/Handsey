using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey
{
    public interface IHandlerResolver
    {
        bool ResolveAll<TType>();

        bool Register<TType>(IList<Type> types);
    }
}