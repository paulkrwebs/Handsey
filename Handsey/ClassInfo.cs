using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Handsey
{
    public class ClassInfo : TypeInfo
    {
        public TypeInfo[] FilteredInterfaces { get; set; }
    }
}