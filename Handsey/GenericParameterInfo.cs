using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Handsey
{
    public class GenericParameterInfo
    {
        /// <summary>
        /// Generic constraints filtered to include class and interface constrains
        /// </summary>
        public IList<ClassInfo> FilteredContraints { get; set; }

        public string Name { get; set; }

        public Type Type { get; set; }
    }
}