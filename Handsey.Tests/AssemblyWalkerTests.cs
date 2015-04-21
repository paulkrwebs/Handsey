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

            // this should really search for another interface so the names don't keep changing in the assert
            Type[] types = assemblyWalker.ListAllTypes(typeof(IHandler), new[] { "Handsey.Tests" });

            Assert.That(types.Count(), Is.EqualTo(11));
        }

        public void ListAllTypes_TypeAndStringArray_NoImplementingTypesToArrayEmpty()
        {
            IAssemblyWalker assemblyWalker = new AssemblyWalker();

            Type[] types = assemblyWalker.ListAllTypes(typeof(INoHandle), new[] { "Handsey.Tests" });

            Assert.That(types.Count(), Is.EqualTo(0));
        }

        #region // stub objects

        private interface INoHandle { }

        #endregion // stub objects
    }
}