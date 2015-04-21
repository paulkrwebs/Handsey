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
    public class ApplicationTests
    {
        private Mock<IAssemblyWalker> _assemblyWalker;
        private Mock<IHandlerFactory> _handlerFactory;
        private Mock<IHandlerSearch> _handlerSearch;
        private Mock<IHandlersSort> _handlerSort;
        private Mock<ITypeConstructor> _typeConstructor;
        private Mock<IApplicationConfiguration> _applicationConfiguration;
        private Mock<IIocContainer> _iocContainer;
        private Mock<IApplicationHandlersFactory> _applicationHandlersFactory;
        private Mock<ApplicationHandlers> _applicationHandlers;
        private IApplicaton _application;

        [SetUp]
        public void Setup()
        {
            _assemblyWalker = new Mock<IAssemblyWalker>();
            _handlerFactory = new Mock<IHandlerFactory>();
            _handlerSearch = new Mock<IHandlerSearch>();
            _handlerSort = new Mock<IHandlersSort>();
            _typeConstructor = new Mock<ITypeConstructor>();
            _applicationConfiguration = new Mock<IApplicationConfiguration>();
            _iocContainer = new Mock<IIocContainer>();
            _applicationHandlersFactory = new Mock<IApplicationHandlersFactory>();
            _applicationHandlers = new Mock<ApplicationHandlers>(new Mock<IList<HandlerInfo>>().Object);

            _application = new Application(_assemblyWalker.Object
                , _handlerFactory.Object
                , _handlerSearch.Object
                , _handlerSort.Object
                , _typeConstructor.Object
                , _applicationHandlersFactory.Object);

            _applicationConfiguration.SetupAllProperties();
            _applicationConfiguration.SetupGet(a => a.IocConatainer).Returns(_iocContainer.Object);
            _applicationConfiguration.SetupGet(a => a.BaseType).Returns(typeof(object));
            _applicationConfiguration.SetupGet(a => a.AssemblyNamePrefixes).Returns(new string[1] { "handsey." });

            _applicationHandlersFactory.Setup(a => a.Create(It.IsAny<IList<HandlerInfo>>()))
                .Returns(_applicationHandlers.Object);

            _application.ApplicationConfiguration = _applicationConfiguration.Object;
        }

        [Test]
        public void Initialise_NoParams_ConfigurationNotSetSoExceptionThrown()
        {
            _application.ApplicationConfiguration = null;

            Assert.That(() => _application.Initialise(), Throws.Exception.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Initialise_NoParams_IocContainerOnConfigurationNotSetSoExceptionThrown()
        {
            _applicationConfiguration.SetupGet(a => a.IocConatainer).Returns(null as IIocContainer);

            Assert.That(() => _application.Initialise(), Throws.Exception.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Initialise_NoParams_AssemblyPrefixOrBaseTypeNotBeenSetSoExceptionThrown()
        {
            _applicationConfiguration.SetupGet(a => a.BaseType).Returns(null as Type);
            _applicationConfiguration.SetupGet(a => a.AssemblyNamePrefixes).Returns(null as string[]);

            Assert.That(() => _application.Initialise(), Throws.Exception.TypeOf<ArgumentException>());

            _applicationConfiguration.SetupGet(a => a.BaseType).Returns(typeof(string));
            _applicationConfiguration.SetupGet(a => a.AssemblyNamePrefixes).Returns(null as string[]);

            Assert.That(() => _application.Initialise(), Throws.Exception.TypeOf<ArgumentException>());

            _applicationConfiguration.SetupGet(a => a.BaseType).Returns(null as Type);
            _applicationConfiguration.SetupGet(a => a.AssemblyNamePrefixes).Returns(new string[1] { "test" });

            Assert.That(() => _application.Initialise(), Throws.Exception.TypeOf<ArgumentException>());
        }

        [Test]
        public void Initialise_NoParams_ApplicationInitialisedCorrectly()
        {
            _application.Initialise();

            _handlerFactory.Setup(h => h.Create(It.IsAny<Type>(), It.IsAny<Type[]>()))
                .Returns(new List<HandlerInfo>() { new HandlerInfo() });

            _assemblyWalker.Verify(a => a.ListAllTypes(It.IsAny<Type>(), It.IsAny<string[]>()), Times.Once());
            _handlerFactory.Verify(h => h.Create(It.IsAny<Type>(), It.IsAny<Type[]>()), Times.Once());
            Assert.That(_application.ApplicationHandlers, Is.Not.Null);
        }

        [Test]
        public void Invoke_Action_InitialisedNotCalledOnObjectSoThrowException()
        {
            Assert.That(() => _application.Invoke<String>((s) => s.ToString()), Throws.Exception.TypeOf<InvalidOperationException>());
        }

        [Test]
        public void Invoke_Action_HandlerResolvesSoTriggerCalledOnEachHandler()
        {
            Mock<Action<Mock<IHandler>>> trigger = new Mock<Action<Mock<IHandler>>>();

            _iocContainer.Setup(i => i.ResolveAll<Mock<IHandler>>()).
                Returns(new Mock<IHandler>[2] { new Mock<IHandler>(), new Mock<IHandler>() });

            _application.Initialise();
            _application.Invoke<Mock<IHandler>>(trigger.Object);

            trigger.Verify(t => t(It.IsAny<Mock<IHandler>>()), Times.Exactly(2));
        }
    }
}