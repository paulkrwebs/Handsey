using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey
{
    public interface ITypeConstructor
    {
        IEnumerable<Type> Create(TypeInfo constructedFrom, IList<TypeInfo> toBeConstructued);

        Type Create(TypeInfo constructedFrom, TypeInfo toBeConstructued);
    }
}