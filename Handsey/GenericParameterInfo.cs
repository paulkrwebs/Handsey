﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Handsey
{
    public class GenericParameterInfo : TypeInfo
    {
        /// <summary>
        /// Generic constraints filtered to include class and interface constrains
        /// </summary>
        public IList<TypeInfo> FilteredContraints { get; set; }

        public string Name { get; set; }
    }
}