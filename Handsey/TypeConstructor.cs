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
        private readonly IHandlerSearch _handlerSearch;

        public TypeConstructor(IHandlerSearch handlerSearch)
        {
            _handlerSearch = handlerSearch;
        }

        public IEnumerable<Type> Create(HandlerInfo constructedFrom, IList<HandlerInfo> toBeConstructued)
        {
            PerformCheck.IsNull(constructedFrom)
                .Throw<ArgumentNullException>(() =>
                    new ArgumentNullException("constructedFrom parameter cannot be null")
                    );

            PerformCheck.IsNull(toBeConstructued)
                .Throw<ArgumentNullException>(() =>
                    new ArgumentNullException("toBeConstructued parameter cannot be null")
                    );

            PerformCheck.IsNull(toBeConstructued.Select(x => x.Type).ToArray())
                .Throw<ArgumentNullException>(() =>
                    new ArgumentNullException("toBeConstructued must have a type")
                    );

            foreach (HandlerInfo typeInfo in toBeConstructued)
            {
                yield return Create(constructedFrom, typeInfo);
            }
        }

        public Type Create(HandlerInfo constructedFrom, HandlerInfo toBeConstructued)
        {
            PerformCheck.IsNull(constructedFrom)
                .Throw<ArgumentNullException>(() =>
                    new ArgumentNullException("constructedFrom parameter cannot be null")
                    );

            PerformCheck.IsNull(toBeConstructued)
                .Throw<ArgumentNullException>(() =>
                    new ArgumentNullException("toBeConstructued parameter cannot be null")
                    );

            PerformCheck.IsNull(toBeConstructued.Type)
                .Throw<ArgumentNullException>(() =>
                    new ArgumentNullException("toBeConstructued must have a type")
                    );

            if (toBeConstructued.IsConstructed)
                return toBeConstructued.Type;

            return ConstructType(constructedFrom, toBeConstructued);
        }

        private Type ConstructType(HandlerInfo constructedFrom, HandlerInfo toBeConstructued)
        {
            PerformCheck.IsNull(constructedFrom.GenericParametersInfo)
                .Throw<ArgumentException>(() =>
                    new ArgumentException("GenericParametersInfo cannot be null")
                    );

            PerformCheck.IsNull(toBeConstructued.GenericParametersInfo)
                .Throw<ArgumentException>(() =>
                    new ArgumentException("GenericParametersInfo cannot be null")
                    );

            if (!constructedFrom.IsInterface)
            {
                PerformCheck.IsTrue(() => constructedFrom.GenericParametersInfo.Count != toBeConstructued.GenericParametersInfo.Count).Throw<ArgumentException>(() => new ArgumentException("Constructed Type cannot be created, the number of generic parameters do not match."));
                return ConstructWithTypes(toBeConstructued, CreateTypeArray(constructedFrom));
            }

            return ConstructFromInterface(constructedFrom, toBeConstructued);
        }

        private Type ConstructFromInterface(HandlerInfo constructedFrom, HandlerInfo toBeConstructued)
        {
            // find interface
            HandlerInfo matchingInteface = _handlerSearch.FindMatchingGenericInterface(constructedFrom, toBeConstructued);

            // match interface paramas to type to be constructed parameters
            return ConstructGenericType(constructedFrom, toBeConstructued, matchingInteface);
        }

        private Type ConstructGenericType(HandlerInfo constructedFrom, TypeInfo toBeConstructued, HandlerInfo matchingInteface)
        {
            List<Type> constructedParamerterTypes = new List<Type>();

            foreach (GenericParameterInfo genericParameterInfo in toBeConstructued.GenericParametersInfo)
            {
                // In a real system this will not occur but including for completeness
                PerformCheck.IsTrue(() => !matchingInteface.ConcreteNestedGenericParametersInfo.ContainsKey(genericParameterInfo.Name))
                    .Throw<KeyNotFoundException>(() =>
                        new KeyNotFoundException("Handler parameters and its own interface parameter names do not match.")
                        );

                GenericParameterInfo matchingInterfaceParameter = matchingInteface.ConcreteNestedGenericParametersInfo[genericParameterInfo.Name];

                PerformCheck.IsTrue(() => constructedFrom.ConcreteNestedGenericParametersInfo.Count() <= matchingInterfaceParameter.Position)
                    .Throw<IndexOutOfRangeException>(() =>
                        new IndexOutOfRangeException("Requested handler and its matched interface on found handler have incorrect number of generic parameters")
                        );

                constructedParamerterTypes.Add(constructedFrom.ConcreteNestedGenericParametersInfo.ElementAt(matchingInterfaceParameter.Position).Value.Type);
            }

            return ConstructWithTypes(toBeConstructued, constructedParamerterTypes.ToArray());
        }

        private static Type ConstructWithTypes(TypeInfo typeInfo, Type[] types)
        {
            return typeInfo.Type.MakeGenericType(types);
        }

        private static Type[] CreateTypeArray(HandlerInfo constructedFrom)
        {
            return constructedFrom.GenericParametersInfo.Select(p => p.Type).ToArray();
        }
    }
}