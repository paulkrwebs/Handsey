using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Handsey
{
    public class TypeInfo
    {
        public IList<GenericParameterInfo> GenericParametersInfo { get; set; }

        public bool IsConstructed { get; set; }

        public bool IsGenericType { get; set; }

        public Type Type { get; set; }

        public Type GenericTypeDefinition { get; set; }

        public TypeInfo[] FilteredInterfaces { get; set; }
    }
}