﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey
{
    public class HandlerInfo : TypeInfo
    {
        public ExecutionOrder ExecutionOrder { get; set; }

        public Type[] ExecutesAfter { get; set; }

        public IList<GenericParameterInfo> ConcreteNestedGenericParametersInfo { get; set; }

        public string GenericSignature { get; set; }
    }
}