using Handsey.Handlers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey.Tests
{
    [TestFixture]
    public class AssemblyWalkerTests
    {
        [Test]
        public void ListAllTypes_TypeAndStringArray_FindsAllImplementingTypes()
        {
            IAssemblyWalker assemblyWalker = new AssemblyWalker();

            Type[] types = assemblyWalker.ListAllTypes(typeof(IHandles), new[] { "Handsey.Tests" });

            Assert.That(types.Count(), Is.EqualTo(3));
        }

        public void ListAllTypes_TypeAndStringArray_NoImplementingTypesToArrayEmpty()
        {
            IAssemblyWalker assemblyWalker = new AssemblyWalker();

            Type[] types = assemblyWalker.ListAllTypes(typeof(INoHandle), new[] { "Handsey.Tests" });

            Assert.That(types.Count(), Is.EqualTo(0));
        }

        #region // stub objects

        interface INoHandle { } 

        #endregion
    }
}
