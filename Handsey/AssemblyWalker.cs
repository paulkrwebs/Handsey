using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey
{
    public class AssemblyWalker : IAssemblyWalker
    {
        public Type[] ListAllTypes(Type baseType, string[] assemblyNamePrefixes)
        {
            return FindTypesImplementing(baseType, assemblyNamePrefixes);
        }

        private Type[] FindTypesImplementing(Type baseType, string[] assemblyNamePrefixes)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                        .Where(a => assemblyNamePrefixes.Any(p => a.FullName.StartsWith(p)))
                        .SelectMany(s => s.GetTypes())
                        .Where(p => baseType.IsAssignableFrom(p) && p.IsClass).ToArray();
        }
    }
}
