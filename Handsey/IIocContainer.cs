using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey
{
    public interface IIocContainer
    {
        void Register(Type from, Type to);

        void Register(Type from, Type to, string name);

        TResolve[] ResolveAll<TResolve>();
    }
}
