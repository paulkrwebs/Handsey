using Handsey.Handlers;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey.Tests
{
    [TestFixture]
    public class ApplicationHandlersTest
    {
        [Test]
        public void Find_HandlerInfoAndHandlerSearch_SearchClassesAreNullSoExceptionThrown()
        {
            Assert.That(() => new ApplicationHandlers(null), Throws.Exception.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Find_HandlerInfoAndHandlerSearch_HandlerSearchIsNullSoExceptionThrown()
        {
            List<HandlerInfo> handles = new List<HandlerInfo>();
            Mock<IHandlerSearch> handlerSearch = new Mock<IHandlerSearch>();
            ApplicationHandlers applicationHandles = new ApplicationHandlers(handles);

            Assert.That(() => applicationHandles.Find(null, handlerSearch.Object), Throws.Exception.TypeOf<ArgumentNullException>(), "Type to search for must have a type");
            Assert.That(() => applicationHandles.Find(new HandlerInfo(), handlerSearch.Object), Throws.Exception.TypeOf<ArgumentNullException>(), "Type to search for must have a type");
            Assert.That(() => applicationHandles.Find(new HandlerInfo() { Type = this.GetType() }, null), Throws.Exception.TypeOf<ArgumentNullException>(), "Search handler cannot be null");
        }

        [Test]
        public void Find_HandlerInfoAndHandlerSearch_HandlerSearchIsCorrectlyCalled()
        {
            List<HandlerInfo> handles = new List<HandlerInfo>();
            Mock<IHandlerSearch> handlerSearch = new Mock<IHandlerSearch>();
            ApplicationHandlers applicationHandles = new ApplicationHandlers(handles);

            applicationHandles.Find(new HandlerInfo() { Type = this.GetType() }, handlerSearch.Object);

            handlerSearch.Verify(h => h.Execute(It.IsAny<HandlerInfo>(), It.IsAny<ConcurrentQueue<HandlerInfo>>()), Times.Once(), "Excute not called on handler search");
        }
    }
}