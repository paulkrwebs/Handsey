using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey
{
    public class HandlerFactory : IHandlerFactory
    {
        private Type _handlerBaseType;

        public IList<TypeInfo> Create(Type[] types)
        {
            return Create(_handlerBaseType, types);
        }

        public IList<TypeInfo> Create(Type handlerBaseType, Type[] types)
        {
            _handlerBaseType = handlerBaseType;

            if (types == null)
                return new List<TypeInfo>();

            if (types.Count() == 0)
                return new List<TypeInfo>();

            return types.Select(t => Create(t)).ToList();
        }

        private TypeInfo Create(Type type)
        {
            return Create(_handlerBaseType, type);
        }

        public TypeInfo Create(Type handlerBaseType, Type type)
        {
            _handlerBaseType = handlerBaseType;

            if (type == null)
                return null;

            return CreateTypeInfo(type);
        }

        private TypeInfo CreateTypeInfo(Type type)
        {
            return CreateTypeInfo<TypeInfo>(type);
        }

        private TypeInfo[] CreateTypeInfo(Type[] types)
        {
            return types
                .Where(t => t != _handlerBaseType)
                .Select(t => CreateTypeInfo(t))
                .ToArray();
        }

        private TTypeInfo CreateTypeInfo<TTypeInfo>(Type type)
            where TTypeInfo : TypeInfo, new()
        {
            return new TTypeInfo()
            {
                IsGenericType = type.IsGenericType,
                IsConstructed = IsConstructed(type),
                Type = type,
                GenericTypeDefinition = CreateGenericTypeDefinition(type),
                GenericParametersInfo = CreateGenericParameters(type),
                FilteredInterfaces = CreateTypeInfo(ListFilteredInterfaces(type))
            };
        }

        private Type CreateGenericTypeDefinition(Type type)
        {
            if (!type.IsGenericType)
                return null;

            return type.GetGenericTypeDefinition();
        }

        private IList<GenericParameterInfo> CreateGenericParameters(Type type)
        {
            return type.GetGenericArguments()
                .Where(t => t.IsClass)
                .Select(t => CreateGenericParameterInfo(t))
                .ToList();
        }

        private GenericParameterInfo CreateGenericParameterInfo(Type type)
        {
            GenericParameterInfo genericParameterInfo = CreateTypeInfo<GenericParameterInfo>(type);
            genericParameterInfo.Name = type.Name;
            genericParameterInfo.FilteredContraints = Create(ListFilteredContraints(type));

            return genericParameterInfo;
        }

        private static bool IsConstructed(Type type)
        {
            if (type.IsGenericType && type.IsConstructedGenericType)
                return true;

            // if its not a generic type then it is constructed
            return !type.IsGenericType;
        }

        private Type[] ListFilteredInterfaces(Type type)
        {
            return type.GetInterfaces()
                .Where(i => _handlerBaseType.IsAssignableFrom(type) && i != _handlerBaseType && i.IsInterface)
                .ToArray();
        }

        private Type[] ListFilteredContraints(Type type)
        {
            if (!type.IsGenericParameter)
                return new Type[0];

            return type.GetGenericParameterConstraints()
                .Where(i => i.IsClass || i.IsInterface)
                .ToArray();
        }
    }
}