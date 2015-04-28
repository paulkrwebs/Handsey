using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey
{
    // interface needs to be immutable
    public interface IApplicationConfiguration
    {
        Type BaseType { get; }

        string[] AssemblyNamePrefixes { get; }
    }
}