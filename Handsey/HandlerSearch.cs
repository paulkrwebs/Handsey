using Handsey.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Handsey
{
    public class HandlerSearch : IHandlerSearch
    {
        private static readonly ConcurrentQueue<ValidMatch> _validMAtches;

        private delegate bool ValidMatch(HandlerInfo a, HandlerInfo b);

        static HandlerSearch()
        {
            _validMAtches = new ConcurrentQueue<ValidMatch>();
            _validMAtches.Enqueue(ExactMatch);
            _validMAtches.Enqueue(Assignable);
            _validMAtches.Enqueue(GenericInterfaceMatch);
            _validMAtches.Enqueue(GenericParamtersAssignable);
        }

        public IEnumerable<HandlerInfo> Execute(HandlerInfo toMatch, IEnumerable<HandlerInfo> listToSearch)
        {
            if (PerformCheck.IsNull(toMatch, listToSearch).Eval())
                yield break;

            foreach (HandlerInfo typeInfo in listToSearch)
            {
                if (Compare(toMatch, typeInfo))
                    yield return typeInfo;
            }
        }

        public bool Compare(HandlerInfo a, HandlerInfo b)
        {
            if (PerformCheck.IsNull(() => a, () => b, () => a.Type, () => b.Type).Eval())
                return false;

            return MatchedAgainstRules(a, b);
        }

        private bool MatchedAgainstRules(HandlerInfo a, HandlerInfo b)
        {
            foreach (ValidMatch validMatch in _validMAtches)
            {
                if (validMatch(a, b))
                    return true;
            }

            return false;
        }

        private static bool ExactMatch(HandlerInfo a, HandlerInfo b)
        {
            return a.Type == b.Type;
        }

        private static bool Assignable(HandlerInfo a, HandlerInfo b)
        {
            return Assignable(a as TypeInfo, b as TypeInfo);
        }

        private static bool Assignable(TypeInfo a, TypeInfo b)
        {
            return a.Type.IsAssignableFrom(b.Type);
        }

        private static bool GenericInterfaceMatch(HandlerInfo a, HandlerInfo b)
        {
            // rework to find an exactly matching interface... If return is null then it doesn't match
            // then expose another method that returns exactly matching interface so I can call from TypeConstructor
            if (FindMatchingGenericInterface(a, b) == null)
                return false;

            return true;
        }

        HandlerInfo IHandlerSearch.FindMatchingGenericInterface(HandlerInfo interfaceHandler, HandlerInfo handlerToSearch)
        {
            return FindMatchingGenericInterface(interfaceHandler, handlerToSearch);
        }

        /// <summary>
        /// Returns the matching handler interface from the handlerToSearch that matches the sourceHandler
        /// </summary>
        /// <param name="handlerInterface"></param>
        /// <param name="handlerToSearch"></param>
        /// <returns></returns>
        private static HandlerInfo FindMatchingGenericInterface(HandlerInfo interfaceHandler, HandlerInfo handlerToSearch)
        {
            if (PerformCheck.IsNull(handlerToSearch.FilteredInterfaces).Eval())
                return null;

            if (!IsGenericInterface(interfaceHandler))
                return null;

            // There can only be one interface of any given type on a class so its safe to find the first (as it will be the only one)
            return handlerToSearch.FilteredInterfaces
                            .FirstOrDefault(i => i.IsGenericType
                                && i.GenericTypeDefinition == interfaceHandler.GenericTypeDefinition
                                && HandlerCanBeConstructedByConcreteNestedGenericParameters(interfaceHandler, handlerToSearch, i)
                                );
        }

        private static bool IsGenericInterface(HandlerInfo sourceHandler)
        {
            if (!sourceHandler.IsInterface)
                return false;

            if (!sourceHandler.IsGenericType)
                return false;

            return true;
        }

        private static bool HandlerCanBeConstructedByConcreteNestedGenericParameters(HandlerInfo sourceHandler, HandlerInfo toConstruct, HandlerInfo matchedInterface)
        {
            // Find the matching generic parameter on the object for this interface
            // once we have that generic parameter we will have all the generic contraints needed to workout
            // if this handler is a match
            foreach (GenericParameterInfo interfaceGenericParameterInfo in matchedInterface.ConcreteNestedGenericParametersInfo)
            {
                // This check is for completeness but will probably never occur in a real system as
                // a developer cannot implement an interface on a type without the generic parameters being present
                GenericParameterInfo compareToGenericParameterInfo =
                    toConstruct.ConcreteNestedGenericParametersInfo.FirstOrDefault(c => c.Name == interfaceGenericParameterInfo.Name);
                if (compareToGenericParameterInfo == null)
                    return false;

                if (!ConcreteNestedGenericParameterMatches(sourceHandler, compareToGenericParameterInfo))
                    return false;
            }

            return true;
        }

        private static bool ConcreteNestedGenericParameterMatches(HandlerInfo sourceHandler, GenericParameterInfo compareToGenericParameterInfo)
        {
            if (sourceHandler.ConcreteNestedGenericParametersInfo.Count() <= compareToGenericParameterInfo.Position)
                return false;

            if (GenericParameterAssignable(sourceHandler.ConcreteNestedGenericParametersInfo[compareToGenericParameterInfo.Position], compareToGenericParameterInfo))
                return true;

            return false;
        }

        private static bool GenericParamtersAssignable(HandlerInfo a, HandlerInfo b)
        {
            if (!DoesHaveEnoughtGenericParametersToConstruct(a, b))
                return false;

            // The order of the generic parameters passed in is important, they must be in the same order as the handler. This is the same as Generics in C#
            for (int i = 0; i < a.GenericParametersInfo.Count; i++)
            {
                if (!GenericParameterAssignable(a.GenericParametersInfo[i], b.GenericParametersInfo[i]))
                    return false;
            }

            return true;
        }

        private static bool DoesHaveEnoughtGenericParametersToConstruct(TypeInfo a, TypeInfo b)
        {
            if (PerformCheck.IsNull(a.GenericParametersInfo, b.GenericParametersInfo).Eval())
                return false;

            return b.GenericParametersInfo.Count() == a.GenericParametersInfo.Count;
        }

        private static bool GenericParameterAssignable(GenericParameterInfo aGenParam, GenericParameterInfo bGenParam)
        {
            // Test exact generic contraint type assignable
            if (aGenParam.Type == bGenParam.Type)
                return true;

            if (AnyGenericParametersContraintAssignable(aGenParam, bGenParam))
                return true;

            return false;
        }

        private static bool AnyGenericParametersContraintAssignable(GenericParameterInfo aGenericParameterInfo, GenericParameterInfo bGenericParameterInfo)
        {
            if (PerformCheck.IsNull(bGenericParameterInfo.FilteredContraints).Eval())
                return false;

            if (!SpecialConstraintsMatched(aGenericParameterInfo, bGenericParameterInfo))
                return false;

            // aGenericParameterType must fullfill ALL contraints for the handler bGenericParameterInfo
            return bGenericParameterInfo.FilteredContraints.All(fc => FilteredConstraintMatched(aGenericParameterInfo, fc));
        }

        private static bool SpecialConstraintsMatched(GenericParameterInfo aGenericParameterInfo, GenericParameterInfo bGenericParameterInfo)
        {
            if (ReferenceTypeConstraintRequiredAndMatched(aGenericParameterInfo, bGenericParameterInfo))
                return false;

            if (DefaultConstructorContraintRequiredAndMatched(aGenericParameterInfo, bGenericParameterInfo))
                return false;

            return true;
        }

        private static bool DefaultConstructorContraintRequiredAndMatched(GenericParameterInfo aGenericParameterInfo, GenericParameterInfo bGenericParameterInfo)
        {
            return bGenericParameterInfo.SpecialConstraint == GenericParameterAttributes.DefaultConstructorConstraint
                            && !aGenericParameterInfo.HasDefaultConstuctor;
        }

        private static bool ReferenceTypeConstraintRequiredAndMatched(GenericParameterInfo aGenericParameterInfo, GenericParameterInfo bGenericParameterInfo)
        {
            return bGenericParameterInfo.SpecialConstraint == GenericParameterAttributes.ReferenceTypeConstraint
                            && aGenericParameterInfo.IsValueType;
        }

        private static bool FilteredConstraintMatched(TypeInfo aType, TypeInfo bType)
        {
            // Check if the generic parameter has a constraint that is assignable from b's generic parameter
            if (Assignable(bType, aType))
                return true;

            if (GenericTypeDefintionMatches(aType, bType))
                return true;

            return false;
        }

        private static bool GenericTypeDefintionMatches(TypeInfo aGenParam, TypeInfo bGenParam)
        {
            if (!bGenParam.IsGenericType)
                return false;

            // If a generic type isn't constructed then any contraints that need to be passed
            // to it will have to be added to the current class. If they aren't the code doesn't compile
            // so if the parameter is a generic type and its not constructed we just need to check that
            if (bGenParam.IsConstructed)
                return false;

            if (aGenParam.GenericTypeDefinition != bGenParam.GenericTypeDefinition)
                return false;

            return true;
        }
    }
}