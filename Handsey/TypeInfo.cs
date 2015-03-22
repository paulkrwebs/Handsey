using System;

namespace Handsey
{
    public class TypeInfo
    {
        public GenericParameterInfo[] GenericParameterInfo { get; set; }

        public bool IsConstructed { get; set; }

        public bool IsGenericType { get; set; }

        public Type Type { get; set; }
    }
}
