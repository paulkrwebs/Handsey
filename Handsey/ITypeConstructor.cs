using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey
{
    public interface ITypeConstructor
    {
        IEnumerable<Type> Create(HandlerInfo constructedFrom, IList<HandlerInfo> toBeConstructued);

        Type Create(HandlerInfo constructedFrom, HandlerInfo toBeConstructued);
    }
}