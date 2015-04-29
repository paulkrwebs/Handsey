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
    public class ApplicationHandlersFactoryTests
    {
        private Mock<IAssemblyWalker> _assemblyWalker;
        private Mock<IHandlerFactory> _handlerFactory;
        private Mock<IApplicationConfiguration> _applicationConfiguration;
        private IApplicationHandlersFactory _applicationHandlersFactory;

        [SetUp]
        public void Setup()
        {
            _assemblyWalker = new Mock<IAssemblyWalker>();
            _handlerFactory = new Mock<IHandlerFactory>();
            _applicationConfiguration = new Mock<IApplicationConfiguration>();

            _applicationHandlersFactory = new ApplicationHandlersFactory(_assemblyWalker.Object
                , _handlerFactory.Object);

            _applicationConfiguration.SetupGet(a => a.BaseType).Returns(typeof(object));
            _applicationConfiguration.SetupGet(a => a.AssemblyNamePrefixes).Returns(new string[1] { "handsey." });
        }

        [Test]
        public void Create_IApplicationConfiguration_ConfigurationNotSetSoExceptionThrown()
        {
            Assert.That(() => _applicationHandlersFactory.Create(null), Throws.Exception.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Create_IApplicationConfiguration_AssemblyPrefixOrBaseTypeNotBeenSetSoExceptionThrown()
        {
            _applicationConfiguration.SetupGet(a => a.BaseType).Returns(null as Type);
            _applicationConfiguration.SetupGet(a => a.AssemblyNamePrefixes).Returns(null as string[]);

            Assert.That(() => _applicationHandlersFactory.Create(_applicationConfiguration.Object), Throws.Exception.TypeOf<ArgumentException>());

            _applicationConfiguration.SetupGet(a => a.BaseType).Returns(typeof(string));
            _applicationConfiguration.SetupGet(a => a.AssemblyNamePrefixes).Returns(null as string[]);

            Assert.That(() => _applicationHandlersFactory.Create(_applicationConfiguration.Object), Throws.Exception.TypeOf<ArgumentException>());

            _applicationConfiguration.SetupGet(a => a.BaseType).Returns(null as Type);
            _applicationConfiguration.SetupGet(a => a.AssemblyNamePrefixes).Returns(new string[1] { "test" });

            Assert.That(() => _applicationHandlersFactory.Create(_applicationConfiguration.Object), Throws.Exception.TypeOf<ArgumentException>());
        }

        [Test]
        public void Create_IApplicationConfiguration_AssemblyWalkerReturnsNullSoHandlersNotFoundExceptionThrown()
        {
            _assemblyWalker.Setup(a => a.ListAllTypes(It.IsAny<Type>(), It.IsAny<string[]>())).Returns<Type[]>(null);

            Assert.That(() => _applicationHandlersFactory.Create(_applicationConfiguration.Object), Throws.Exception.TypeOf<HandlerNotFoundException>());

            _assemblyWalker.Verify(a => a.ListAllTypes(It.IsAny<Type>(), It.IsAny<string[]>()), Times.Once());
            _handlerFactory.Verify(h => h.Create(It.IsAny<Type[]>()), Times.Never());
        }

        [Test]
        public void Create_IApplicationConfiguration_AssemblyWalkerReturnsEmptyArraySoHandlersNotFoundExceptionThrown()
        {
            _assemblyWalker.Setup(a => a.ListAllTypes(It.IsAny<Type>(), It.IsAny<string[]>())).Returns(new Type[0]);

            Assert.That(() => _applicationHandlersFactory.Create(_applicationConfiguration.Object), Throws.Exception.TypeOf<HandlerNotFoundException>());

            _assemblyWalker.Verify(a => a.ListAllTypes(It.IsAny<Type>(), It.IsAny<string[]>()), Times.Once());
            _handlerFactory.Verify(h => h.Create(It.IsAny<Type[]>()), Times.Never());
        }

        [Test]
        public void Create_IApplicationConfiguration_HandlerFactoryReturnsNullSoHandlersNotFoundExceptionThrown()
        {
            _assemblyWalker.Setup(a => a.ListAllTypes(It.IsAny<Type>(), It.IsAny<string[]>())).Returns(new Type[1] { typeof(IHandler) });
            _handlerFactory.Setup(h => h.Create(It.IsAny<Type[]>())).Returns<List<HandlerInfo>>(null);

            Assert.That(() => _applicationHandlersFactory.Create(_applicationConfiguration.Object), Throws.Exception.TypeOf<HandlerNotFoundException>());

            _assemblyWalker.Verify(a => a.ListAllTypes(It.IsAny<Type>(), It.IsAny<string[]>()), Times.Once());
            _handlerFactory.Verify(h => h.Create(It.IsAny<Type[]>()), Times.Once());
        }

        [Test]
        public void Create_IApplicationConfiguration_HandlerFactoryReturnsEmptyListSoHandlersNotFoundExceptionThrown()
        {
            _assemblyWalker.Setup(a => a.ListAllTypes(It.IsAny<Type>(), It.IsAny<string[]>())).Returns(new Type[1] { typeof(IHandler) });
            _handlerFactory.Setup(h => h.Create(It.IsAny<Type[]>())).Returns(new List<HandlerInfo>());

            Assert.That(() => _applicationHandlersFactory.Create(_applicationConfiguration.Object), Throws.Exception.TypeOf<HandlerNotFoundException>());

            _assemblyWalker.Verify(a => a.ListAllTypes(It.IsAny<Type>(), It.IsAny<string[]>()), Times.Once());
            _handlerFactory.Verify(h => h.Create(It.IsAny<Type[]>()), Times.Once());
        }

        [Test]
        public void Create_IApplicationConfiguration_ApplicationHandlersCreatedCorrectly()
        {
            _assemblyWalker.Setup(a => a.ListAllTypes(It.IsAny<Type>(), It.IsAny<string[]>())).Returns(new Type[1] { typeof(IHandler) });
            _handlerFactory.Setup(h => h.Create(It.IsAny<Type[]>())).Returns(new List<HandlerInfo>() { new HandlerInfo() });

            IApplicationHandlers applicationHandlers = _applicationHandlersFactory.Create(_applicationConfiguration.Object);

            Assert.That(applicationHandlers, Is.Not.Null);
            _assemblyWalker.Verify(a => a.ListAllTypes(It.IsAny<Type>(), It.IsAny<string[]>()), Times.Once());
            _handlerFactory.Verify(h => h.Create(It.IsAny<Type[]>()), Times.Once());
        }
    }
}