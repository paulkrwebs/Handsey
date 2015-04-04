using Handsey.Handlers;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey.Tests
{
    [TestFixture]
    public class ApplicationHandlesTest
    {
        [Test]
        public void Find_HandlerSearch_SearchClassesAreNullSoExceptionThrown()
        {
            Assert.That(() => new ApplicationHandles(null), Throws.Exception.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Find_HandlerSearch_HandlerSearchIsNullSoExceptionThrown()
        {
            List<TypeInfo> handles = new List<TypeInfo>();
            ApplicationHandles applicationHandles = new ApplicationHandles(handles);

            Assert.That(() => applicationHandles.Find(new TypeInfo(), null), Throws.Exception.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Find_HandlerSearch_HandlerSearchIsCorrectlyCalled()
        {
            List<TypeInfo> handles = new List<TypeInfo>();
            Mock<IHandlerSearch> handlerSearch = new Mock<IHandlerSearch>();
            ApplicationHandles applicationHandles = new ApplicationHandles(handles);

            applicationHandles.Find(new TypeInfo(), handlerSearch.Object);

            handlerSearch.Verify(h => h.Execute(It.IsAny<TypeInfo>(), It.Is<IList<TypeInfo>>(hls => hls == handles)), Times.Once(), "Excute not called on handler search");
        }
    }
}