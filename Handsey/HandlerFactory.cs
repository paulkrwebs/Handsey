using Handsey.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Handsey
{
    public class HandlerFactory : IHandlerFactory
    {
        private Type _handlerBaseType;

        public IList<HandlerInfo> Create(Type handlerBaseType, Type[] types)
        {
            _handlerBaseType = handlerBaseType;

            if (types == null)
                return new List<HandlerInfo>();

            if (types.Count() == 0)
                return new List<HandlerInfo>();

            return types.Select(t => Create(t)).ToList();
        }

        private HandlerInfo Create(Type type)
        {
            return Create(_handlerBaseType, type);
        }

        public HandlerInfo Create(Type handlerBaseType, Type type)
        {
            _handlerBaseType = handlerBaseType;

            if (type == null)
                return null;

            return CreateHandlerInfo(type);
        }

        private HandlerInfo CreateHandlerInfo(Type type)
        {
            HandlerInfo handlerInfo = CreateTypeInfo<HandlerInfo>(type);

            PopulateAttributeInfo(type, handlerInfo);

            return handlerInfo;
        }

        private TTypeInfo CreateTypeInfo<TTypeInfo>(Type type)
            where TTypeInfo : TypeInfo, new()
        {
            TTypeInfo typeInfo = new TTypeInfo()
            {
                IsGenericType = type.IsGenericType,
                IsConstructed = IsConstructed(type),
                Type = type,
                GenericTypeDefinition = CreateGenericTypeDefinition(type),
                GenericParametersInfo = CreateGenericParameters(type),
                FilteredInterfaces = CreateTypeInfo(ListFilteredInterfaces(type))
            };

            return typeInfo;
        }

        private void PopulateAttributeInfo(Type type, HandlerInfo typeInfo)
        {
            if (!_handlerBaseType.IsAssignableFrom(type))
                return;

            AttributeCollection attributes = TypeDescriptor.GetAttributes(type);
            List<HandlerAttribute> handlesAttribute = attributes.OfType<HandlerAttribute>().ToList();

            // There can only be one execution type so only take first one
            if (handlesAttribute.Count > 0)
                typeInfo.ExecutionOrder = handlesAttribute[0].ExecutionOrder;

            typeInfo.ExecutesAfter = handlesAttribute.OfType<HandlesAfter>().Select(h => h.Type).ToArray();
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
            genericParameterInfo.FilteredContraints = CreateTypeInfo(ListFilteredContraints(type));
            genericParameterInfo.SpecialConstraint = CreateSpecialConstraints(type);

            return genericParameterInfo;
        }

        private static GenericParameterAttributes CreateSpecialConstraints(Type type)
        {
            if (!type.IsGenericParameter)
                return GenericParameterAttributes.None;

            return type.GenericParameterAttributes & GenericParameterAttributes.SpecialConstraintMask;
        }

        private TypeInfo[] CreateTypeInfo(Type[] types)
        {
            return Create(_handlerBaseType, types).ToArray();
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