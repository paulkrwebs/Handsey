using Handsey.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey
{
    public class TypeConstructor : ITypeConstructor
    {
        public IEnumerable<Type> Create(TypeInfo constructedFrom, IList<TypeInfo> toBeConstructued)
        {
            NullCheck.ThrowIfNull<ArgumentNullException>(constructedFrom, () => new ArgumentNullException("constructedFrom parameter cannot be null"));
            NullCheck.ThrowIfNull<ArgumentNullException>(toBeConstructued, () => new ArgumentNullException("toBeConstructued parameter cannot be null"));
            NullCheck.ThrowIfNull<ArgumentNullException>(() => new ArgumentNullException("toBeConstructued must have a type"), toBeConstructued.Select(x => x.Type).ToArray());

            foreach (TypeInfo typeInfo in toBeConstructued)
            {
                yield return Create(constructedFrom, typeInfo);
            }
        }

        public Type Create(TypeInfo constructedFrom, TypeInfo toBeConstructued)
        {
            NullCheck.ThrowIfNull<ArgumentNullException>(constructedFrom, () => new ArgumentNullException("constructedFrom parameter cannot be null"));
            NullCheck.ThrowIfNull<ArgumentNullException>(toBeConstructued, () => new ArgumentNullException("toBeConstructued parameter cannot be null"));
            NullCheck.ThrowIfNull<ArgumentNullException>(toBeConstructued.Type, () => new ArgumentNullException("toBeConstructued must have a type"));

            if (toBeConstructued.IsConstructed)
                return toBeConstructued.Type;

            return ConstructType(constructedFrom, toBeConstructued);
        }

        private Type ConstructType(TypeInfo constructedFrom, TypeInfo typeInfo)
        {
            NullCheck.ThrowIfNull<ArgumentException>(constructedFrom.GenericParametersInfo, () => new ArgumentException("GenericParametersInfo cannot be null"));
            CheckGenericParameterCountMatches(constructedFrom.GenericParametersInfo, typeInfo.GenericParametersInfo);

            return typeInfo.Type.MakeGenericType(CreateTypeArray(constructedFrom));
        }

        private void CheckGenericParameterCountMatches(IList<GenericParameterInfo> listA, IList<GenericParameterInfo> listB)
        {
            if (listA.Count != listB.Count)
                throw new ArgumentException("Constructed Type cannot be created, the number of generic parameters do not match.");
        }

        private static Type[] CreateTypeArray(TypeInfo constructedFrom)
        {
            return constructedFrom.GenericParametersInfo.Select(p => p.Type).ToArray();
        }
    }
}