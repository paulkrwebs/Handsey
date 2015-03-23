using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey
{
    public interface IApplicationConfiguration
    {
        Type BaseType { get; set; }

        string[] AssemblyNamePrefixes { get; set; }
    }
}