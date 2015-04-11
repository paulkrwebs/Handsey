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
        public void Find_HandlerInfoAndHandlerSearch_SearchClassesAreNullSoExceptionThrown()
        {
            Assert.That(() => new ApplicationHandles(null), Throws.Exception.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Find_HandlerInfoAndHandlerSearch_HandlerSearchIsNullSoExceptionThrown()
        {
            List<HandlerInfo> handles = new List<HandlerInfo>();
            Mock<IHandlerSearch> handlerSearch = new Mock<IHandlerSearch>();
            ApplicationHandles applicationHandles = new ApplicationHandles(handles);

            Assert.That(() => applicationHandles.Find(null, handlerSearch.Object), Throws.Exception.TypeOf<ArgumentNullException>(), "Type to search for must have a type");
            Assert.That(() => applicationHandles.Find(new HandlerInfo(), handlerSearch.Object), Throws.Exception.TypeOf<ArgumentNullException>(), "Type to search for must have a type");
            Assert.That(() => applicationHandles.Find(new HandlerInfo() { Type = this.GetType() }, null), Throws.Exception.TypeOf<ArgumentNullException>(), "Search handler cannot be null");
        }

        [Test]
        public void Find_HandlerInfoAndHandlerSearch_HandlerSearchIsCorrectlyCalled()
        {
            List<HandlerInfo> handles = new List<HandlerInfo>();
            Mock<IHandlerSearch> handlerSearch = new Mock<IHandlerSearch>();
            ApplicationHandles applicationHandles = new ApplicationHandles(handles);

            applicationHandles.Find(new HandlerInfo() { Type = this.GetType() }, handlerSearch.Object);

            handlerSearch.Verify(h => h.Execute(It.IsAny<HandlerInfo>(), It.Is<IList<HandlerInfo>>(hls => hls == handles)), Times.Once(), "Excute not called on handler search");
        }

        [Test]
        public void PreviouslyAttemptedToFind_HandlerInfo_SearchHasBeenAttemptedBEforeSoReturnTrue()
        {
            List<HandlerInfo> handles = new List<HandlerInfo>();
            Mock<IHandlerSearch> handlerSearch = new Mock<IHandlerSearch>();
            ApplicationHandles applicationHandles = new ApplicationHandles(handles);

            applicationHandles.Find(new HandlerInfo() { Type = this.GetType() }, handlerSearch.Object);

            handlerSearch.Verify(h => h.Execute(It.IsAny<HandlerInfo>(), It.Is<IList<HandlerInfo>>(hls => hls == handles)), Times.Once(), "Excute not called on handler search");
        }

        [Test]
        public void PreviouslyAttemptedToFind_HandlerInfo_ToSearhForDoesNotHaveATypeSoThrowException()
        {
            List<HandlerInfo> handles = new List<HandlerInfo>();
            Mock<IHandlerSearch> handlerSearch = new Mock<IHandlerSearch>();
            ApplicationHandles applicationHandles = new ApplicationHandles(handles);

            Assert.That(() => applicationHandles.PreviouslyAttemptedToFind(null), Throws.Exception.TypeOf<ArgumentNullException>(), "Must provide a type info with a type");
            Assert.That(() => applicationHandles.PreviouslyAttemptedToFind(new HandlerInfo()), Throws.Exception.TypeOf<ArgumentNullException>(), "Must provide a type info with a type");
        }

        [Test]
        public void PreviouslyAttemptedToFind_HandlerInfo_SearchHasNotBeenAttemptedBEforeSoReturnFalse()
        {
            List<HandlerInfo> handles = new List<HandlerInfo>();
            Mock<IHandlerSearch> handlerSearch = new Mock<IHandlerSearch>();
            ApplicationHandles applicationHandles = new ApplicationHandles(handles);

            applicationHandles.Find(new HandlerInfo() { Type = typeof(string) }, handlerSearch.Object);

            Assert.That(applicationHandles.PreviouslyAttemptedToFind(new HandlerInfo() { Type = typeof(int) }), Is.False);
        }

        [Test]
        public void ClearPreviousFindAttemptsCache_NoParams_CacheIsClearedCorrectly()
        {
            List<HandlerInfo> handles = new List<HandlerInfo>();
            Mock<IHandlerSearch> handlerSearch = new Mock<IHandlerSearch>();
            ApplicationHandles applicationHandles = new ApplicationHandles(handles);

            applicationHandles.Find(new HandlerInfo() { Type = typeof(int) }, handlerSearch.Object);

            Assert.That(applicationHandles.PreviouslyAttemptedToFind(new HandlerInfo() { Type = typeof(int) }), Is.True);

            applicationHandles.ClearPreviousFindAttemptsCache();

            Assert.That(applicationHandles.PreviouslyAttemptedToFind(new HandlerInfo() { Type = typeof(int) }), Is.False);
        }
    }
}