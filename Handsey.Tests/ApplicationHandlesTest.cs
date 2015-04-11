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
        public void Find_TypeInfoAndHandlerSearch_SearchClassesAreNullSoExceptionThrown()
        {
            Assert.That(() => new ApplicationHandles(null), Throws.Exception.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Find_TypeInfoAndHandlerSearch_HandlerSearchIsNullSoExceptionThrown()
        {
            List<TypeInfo> handles = new List<TypeInfo>();
            Mock<IHandlerSearch> handlerSearch = new Mock<IHandlerSearch>();
            ApplicationHandles applicationHandles = new ApplicationHandles(handles);

            Assert.That(() => applicationHandles.Find(null, handlerSearch.Object), Throws.Exception.TypeOf<ArgumentNullException>(), "Type to search for must have a type");
            Assert.That(() => applicationHandles.Find(new TypeInfo(), handlerSearch.Object), Throws.Exception.TypeOf<ArgumentNullException>(), "Type to search for must have a type");
            Assert.That(() => applicationHandles.Find(new TypeInfo() { Type = this.GetType() }, null), Throws.Exception.TypeOf<ArgumentNullException>(), "Search handler cannot be null");
        }

        [Test]
        public void Find_TypeInfoAndHandlerSearch_HandlerSearchIsCorrectlyCalled()
        {
            List<TypeInfo> handles = new List<TypeInfo>();
            Mock<IHandlerSearch> handlerSearch = new Mock<IHandlerSearch>();
            ApplicationHandles applicationHandles = new ApplicationHandles(handles);

            applicationHandles.Find(new TypeInfo() { Type = this.GetType() }, handlerSearch.Object);

            handlerSearch.Verify(h => h.Execute(It.IsAny<TypeInfo>(), It.Is<IList<TypeInfo>>(hls => hls == handles)), Times.Once(), "Excute not called on handler search");
        }

        [Test]
        public void PreviouslyAttemptedToFind_TypeInfo_SearchHasBeenAttemptedBEforeSoReturnTrue()
        {
            List<TypeInfo> handles = new List<TypeInfo>();
            Mock<IHandlerSearch> handlerSearch = new Mock<IHandlerSearch>();
            ApplicationHandles applicationHandles = new ApplicationHandles(handles);

            applicationHandles.Find(new TypeInfo() { Type = this.GetType() }, handlerSearch.Object);

            handlerSearch.Verify(h => h.Execute(It.IsAny<TypeInfo>(), It.Is<IList<TypeInfo>>(hls => hls == handles)), Times.Once(), "Excute not called on handler search");
        }

        [Test]
        public void PreviouslyAttemptedToFind_TypeInfo_ToSearhForDoesNotHaveATypeSoThrowException()
        {
            List<TypeInfo> handles = new List<TypeInfo>();
            Mock<IHandlerSearch> handlerSearch = new Mock<IHandlerSearch>();
            ApplicationHandles applicationHandles = new ApplicationHandles(handles);

            Assert.That(() => applicationHandles.PreviouslyAttemptedToFind(null), Throws.Exception.TypeOf<ArgumentNullException>(), "Must provide a type info with a type");
            Assert.That(() => applicationHandles.PreviouslyAttemptedToFind(new TypeInfo()), Throws.Exception.TypeOf<ArgumentNullException>(), "Must provide a type info with a type");
        }

        [Test]
        public void PreviouslyAttemptedToFind_TypeInfo_SearchHasNotBeenAttemptedBEforeSoReturnFalse()
        {
            List<TypeInfo> handles = new List<TypeInfo>();
            Mock<IHandlerSearch> handlerSearch = new Mock<IHandlerSearch>();
            ApplicationHandles applicationHandles = new ApplicationHandles(handles);

            applicationHandles.Find(new TypeInfo() { Type = typeof(string) }, handlerSearch.Object);

            Assert.That(applicationHandles.PreviouslyAttemptedToFind(new TypeInfo() { Type = typeof(int) }), Is.False);
        }

        [Test]
        public void ClearPreviousFindAttemptsCache_NoParams_CacheIsClearedCorrectly()
        {
            List<TypeInfo> handles = new List<TypeInfo>();
            Mock<IHandlerSearch> handlerSearch = new Mock<IHandlerSearch>();
            ApplicationHandles applicationHandles = new ApplicationHandles(handles);

            applicationHandles.Find(new TypeInfo() { Type = typeof(int) }, handlerSearch.Object);

            Assert.That(applicationHandles.PreviouslyAttemptedToFind(new TypeInfo() { Type = typeof(int) }), Is.True);

            applicationHandles.ClearPreviousFindAttemptsCache();

            Assert.That(applicationHandles.PreviouslyAttemptedToFind(new TypeInfo() { Type = typeof(int) }), Is.False);
        }
    }
}