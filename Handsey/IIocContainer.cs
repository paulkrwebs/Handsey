using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey
{
    /// <summary>
    /// Methods on implementing containers need to be thread safe.
    /// e.g. Unity Register is not threadsafe
    /// </summary>
    public interface IIocContainer
    {
        void Register(Type from, Type to, string name);

        TResolve[] ResolveAll<TResolve>();
    }
}