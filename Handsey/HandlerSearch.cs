using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey
{
    public class HandlerSearch : IHandlerSearch
    {
        private static readonly IList<ValidMatch> _validMAtches;

        private delegate bool ValidMatch(TypeInfo a, TypeInfo b);

        static HandlerSearch()
        {
            _validMAtches = new List<ValidMatch>()
            {
                IsExactMatch,
                IsAssignable,
                IsGenericParamtersAssignable
            };
        }

        public IList<TypeInfo> Execute(TypeInfo toMatch, IList<TypeInfo> listToSearch)
        {
            throw new NotImplementedException();
        }

        public bool Compare(TypeInfo a, TypeInfo b)
        {
            if (Null(a, b, a.Type, b.Type))
                return false;

            return IsMatchedAgainstRules(a, b);
        }

        private static bool IsMatchedAgainstRules(TypeInfo a, TypeInfo b)
        {
            foreach (ValidMatch validMatch in _validMAtches)
            {
                if (validMatch(a, b))
                    return true;
            }

            return false;
        }

        private static bool IsExactMatch(TypeInfo a, TypeInfo b)
        {
            return a.Type == b.Type;
        }

        private static bool IsAssignable(TypeInfo a, TypeInfo b)
        {
            return a.Type.IsAssignableFrom(b.Type);
        }

        private static bool IsGenericParamtersAssignable(TypeInfo a, TypeInfo b)
        {
            if (!DoesHaveEnoughtGenericParametersToConstruct(a, b))
                return false;

            // The order of the generic parameters passed in is important, they must be in the same order as the handler. This is the same as Generics in C#
            for (int i = 0; i < a.GenericParametersInfo.Count; i++)
            {
                GenericParameterInfo aGenParam = a.GenericParametersInfo[i];
                GenericParameterInfo bGenParam = b.GenericParametersInfo[i];

                if (!IsGenericParameterAssignable(aGenParam, bGenParam))
                    return false;
            }

            return true;
        }

        private static bool DoesHaveEnoughtGenericParametersToConstruct(TypeInfo a, TypeInfo b)
        {
            if (Null(a.GenericParametersInfo, b.GenericParametersInfo))
                return false;

            return b.GenericParametersInfo
                .Where(v => !v.IsConstructed).Count() == a.GenericParametersInfo.Count;
        }

        private static bool IsGenericParameterAssignable(GenericParameterInfo aGenParam, GenericParameterInfo bGenParam)
        {
            // Test exact generic contraint type assignable
            if (aGenParam.Type == bGenParam.Type)
                return true;

            if (IsAnyGenericParametersContraintAssignable(aGenParam, bGenParam))
                return true;

            return false;
        }

        private static bool IsAnyGenericParametersContraintAssignable(GenericParameterInfo aGenericParameterInfo, GenericParameterInfo bGenericParameterInfo)
        {
            if (Null(aGenericParameterInfo.FilteredContraints))
                return false;

            // Check if the generic parameter has a constraint that is assignable from b's generic parameter
            return bGenericParameterInfo.FilteredContraints.Any(fc => fc.Type.IsAssignableFrom(aGenericParameterInfo.Type));
        }

        private static bool Null(params object[] objs)
        {
            return objs.Any(o => o == null);
        }
    }
}