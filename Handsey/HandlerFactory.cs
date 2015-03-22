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

        public IList<ClassInfo> Create(Type[] types)
        {
            return Create(_handlerBaseType, types);
        }

        public IList<ClassInfo> Create(Type handlerBaseType, Type[] types)
        {
            _handlerBaseType = handlerBaseType;

            if (types == null)
                return new List<ClassInfo>();

            if (types.Count() == 0)
                return new List<ClassInfo>();

            return types.Select(t => Create(t)).ToList();
        }

        private ClassInfo Create(Type type)
        {
            return Create(_handlerBaseType, type);
        }

        public ClassInfo Create(Type handlerBaseType, Type type)
        {
            _handlerBaseType = handlerBaseType;

            if (type == null)
                return null;

            return CreateClassInfo(type);
        }

        private ClassInfo CreateClassInfo(Type type)
        {
            return new ClassInfo()
            {
                IsGenericType = type.IsGenericType,
                IsConstructed = IsConstructed(type),
                Type = type,
                FilteredInterfaces = CreateTypeInfo(ListFilteredInterfaces(type)),
                GenericParameterInfo = CreateGenerictParameters(type)
            };
        }

        private TypeInfo[] CreateTypeInfo(Type[] types)
        {
            return types
                .Where(t => t != _handlerBaseType)
                .Select(t => CreateClassInfo(t))
                .ToArray();
        }

        private GenericParameterInfo[] CreateGenerictParameters(Type type)
        {
            return type.GetGenericArguments()
                .Where(t => t.IsClass)
                .Select(t => new GenericParameterInfo() { Type = t, Name = t.Name, FilteredContraints = Create(ListFilteredContraints(t)) })
                .ToArray();
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