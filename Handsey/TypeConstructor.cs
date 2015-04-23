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
        public IEnumerable<Type> Create(HandlerInfo constructedFrom, IList<HandlerInfo> toBeConstructued)
        {
            PerformCheck.IsNull(constructedFrom).Throw<ArgumentNullException>(() => new ArgumentNullException("constructedFrom parameter cannot be null"));
            PerformCheck.IsNull(toBeConstructued).Throw<ArgumentNullException>(() => new ArgumentNullException("toBeConstructued parameter cannot be null"));
            PerformCheck.IsNull(toBeConstructued.Select(x => x.Type).ToArray()).Throw<ArgumentNullException>(() => new ArgumentNullException("toBeConstructued must have a type"));

            foreach (HandlerInfo typeInfo in toBeConstructued)
            {
                yield return Create(constructedFrom, typeInfo);
            }
        }

        public Type Create(HandlerInfo constructedFrom, HandlerInfo toBeConstructued)
        {
            PerformCheck.IsNull(constructedFrom).Throw<ArgumentNullException>(() => new ArgumentNullException("constructedFrom parameter cannot be null"));
            PerformCheck.IsNull(toBeConstructued).Throw<ArgumentNullException>(() => new ArgumentNullException("toBeConstructued parameter cannot be null"));
            PerformCheck.IsNull(toBeConstructued.Type).Throw<ArgumentNullException>(() => new ArgumentNullException("toBeConstructued must have a type"));

            if (toBeConstructued.IsConstructed)
                return toBeConstructued.Type;

            return ConstructType(constructedFrom, toBeConstructued);
        }

        private Type ConstructType(HandlerInfo constructedFrom, HandlerInfo typeInfo)
        {
            PerformCheck.IsNull(constructedFrom.GenericParametersInfo).Throw<ArgumentException>(() => new ArgumentException("GenericParametersInfo cannot be null"));
            CheckGenericParameterCountMatches(constructedFrom.GenericParametersInfo, typeInfo.GenericParametersInfo);

            return typeInfo.Type.MakeGenericType(CreateTypeArray(constructedFrom));
        }

        private void CheckGenericParameterCountMatches(IList<GenericParameterInfo> listA, IList<GenericParameterInfo> listB)
        {
            PerformCheck.IsTrue(() => listA.Count != listB.Count).Throw<ArgumentException>(() => new ArgumentException("Constructed Type cannot be created, the number of generic parameters do not match."));
        }

        private static Type[] CreateTypeArray(HandlerInfo constructedFrom)
        {
            return constructedFrom.GenericParametersInfo.Select(p => p.Type).ToArray();
        }
    }
}