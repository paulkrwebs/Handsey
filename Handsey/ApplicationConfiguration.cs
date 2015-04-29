using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey
{
    public sealed class ApplicationConfiguration : IApplicationConfiguration
    {
        private readonly Type _baseType;
        private readonly string[] _assemblyNamePrefixes;

        public Type BaseType { get { return _baseType; } }

        public string[] AssemblyNamePrefixes { get { return _assemblyNamePrefixes; } }

        public ApplicationConfiguration(Type baseType, string[] assemblyNamePrefixes)
        {
            _baseType = baseType;
            _assemblyNamePrefixes = assemblyNamePrefixes;
        }
    }
}