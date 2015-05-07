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
            _validMAtches.Enqueue(GenericParamtersAssignable);
            _validMAtches.Enqueue(InterfaceMatch);
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

        private static bool MatchedAgainstRules(HandlerInfo a, HandlerInfo b)
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

        private static bool InterfaceMatch(HandlerInfo a, HandlerInfo b)
        {
            if (!a.IsInterface)
                return false;

            if (!DoesImplementGenericInterface(a, b))
                return false;

            return true;
        }

        private static bool DoesImplementGenericInterface(HandlerInfo a, HandlerInfo b)
        {
            if (!a.IsGenericType)
                return false;

            HandlerInfo[] foundInterfaces = FindMatchingInterface(a, b);
            if (!foundInterfaces.Any())
                return false;

            return HandlerConstructableFromInterfaceParameters(a, b, foundInterfaces);
        }

        private static bool HandlerConstructableFromInterfaceParameters(HandlerInfo a, HandlerInfo b, HandlerInfo[] foundInterfaces)
        {
            foreach (HandlerInfo handlerInterface in foundInterfaces)
            {
                foreach (string key in handlerInterface.ConcreteNestedGenericParametersInfo.Keys)
                {
                    GenericParameterInfo genericParameterInfo = null;
                    if (!b.ConcreteNestedGenericParametersInfo.TryGetValue(key, out genericParameterInfo))
                        continue;

                    if (GenericParameterAssignable(a.ConcreteNestedGenericParametersInfo.ElementAt(genericParameterInfo.Position).Value, genericParameterInfo))
                        return true;
                }
            }

            return false;
        }

        private static HandlerInfo[] FindMatchingInterface(HandlerInfo a, HandlerInfo b)
        {
            return b.FilteredInterfaces
                            .Where(i => i.IsGenericType && i.GenericTypeDefinition == a.GenericTypeDefinition)
                            .ToArray();
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