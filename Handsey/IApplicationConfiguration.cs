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
        IIocContainer IocConatainer { get; }

        Type BaseType { get; }

        string[] AssemblyNamePrefixes { get; }

        /// <summary>
        /// Populates the immutable object
        /// </summary>
        /// <param name="iocContainer"></param>
        /// <param name="baseType"></param>
        /// <param name="assemblyNamePrefixes"></param>
        void Populate(IIocContainer iocContainer, Type baseType, string[] assemblyNamePrefixes);
    }
}