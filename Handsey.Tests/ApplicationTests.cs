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
        private Mock<IApplicationHandlers> _applicationHandlers;
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
            _applicationHandlers = new Mock<IApplicationHandlers>();

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

        [Test]
        public void Invoke_Action_HandlerNotRegisteredSoFindHanlderAndReturnListAsNull()
        {
            Mock<Action<Mock<IHandler>>> trigger = new Mock<Action<Mock<IHandler>>>();

            _handlerFactory.Setup(h => h.Create(It.IsAny<Type>(), It.IsAny<Type>()))
                .Returns<HandlerInfo>(null);

            _applicationHandlers.Setup(a => a.Find(It.IsAny<HandlerInfo>(), It.IsAny<IHandlerSearch>()))
                .Returns<IEnumerable<HandlerInfo>>(null);

            _application.Initialise();

            Assert.That(() => _application.Invoke<Mock<IHandler>>(trigger.Object), Throws.Exception.TypeOf<HandlerNotFoundException>());
            trigger.Verify(t => t(It.IsAny<Mock<IHandler>>()), Times.Never);
            _handlerFactory.Verify(h => h.Create(It.IsAny<Type>(), It.Is<Type>(t => t == typeof(Mock<IHandler>))), Times.Once(), "All call create handler once");
            _applicationHandlers.Verify(a => a.Find(It.IsAny<HandlerInfo>(), It.IsAny<IHandlerSearch>()), Times.Once(), "Should only try to find handlers once");
        }

        [Test]
        public void Invoke_Action_HandlerNotRegisteredSoFindHanlderAndReturnListAsEmpty()
        {
            Mock<Action<Mock<IHandler>>> trigger = new Mock<Action<Mock<IHandler>>>();

            _handlerFactory.Setup(h => h.Create(It.IsAny<Type>(), It.IsAny<Type>()))
                .Returns<HandlerInfo>(null);

            _applicationHandlers.Setup(a => a.Find(It.IsAny<HandlerInfo>(), It.IsAny<IHandlerSearch>()))
                .Returns(Enumerable.Empty<HandlerInfo>());

            _application.Initialise();

            Assert.That(() => _application.Invoke<Mock<IHandler>>(trigger.Object), Throws.Exception.TypeOf<HandlerNotFoundException>());
            trigger.Verify(t => t(It.IsAny<Mock<IHandler>>()), Times.Never);
            _handlerFactory.Verify(h => h.Create(It.IsAny<Type>(), It.Is<Type>(t => t == typeof(Mock<IHandler>))), Times.Once(), "All call create handler once");
            _applicationHandlers.Verify(a => a.Find(It.IsAny<HandlerInfo>(), It.IsAny<IHandlerSearch>()), Times.Once(), "Should only try to find handlers once");
        }

        [Test]
        public void Invoke_Action_HanlderNotRegisteredFoundOneHandlerAndConstructAndRegister()
        {
            //Mock<Action<Mock<IHandler>>> trigger = new Mock<Action<Mock<IHandler>>>();

            //_handlerFactory.Setup(h => h.Create(It.IsAny<Type>(), It.IsAny<Type>()))
            //    .Returns(new HandlerInfo());

            //_application.Initialise();
            //_application.Invoke<Mock<IHandler>>(trigger.Object);

            //trigger.Verify(t => t(It.IsAny<Mock<IHandler>>()), Times.Exactly(1));
            //_handlerFactory.Verify(h => h.Create(It.IsAny<Type>(), It.Is<Type>(t => t == typeof(Mock<IHandler>))), Times.Once(), "All call create handler once");
            //_applicationHandlers.Verify(a => a.Find(It.IsAny<HandlerInfo>(), It.IsAny<IHandlerSearch>()), Times.Once(), "Should only try to find handlers once");
            //_handlerSort.Verify(h => h.Sort(It.IsAny<IEnumerable<HandlerInfo>>()), Times.Never(), "Only one handler so doesn't need to be sorted");
            //_typeConstructor.Verify(t => t.Create(It.IsAny<HandlerInfo>(), It.IsAny<HandlerInfo>()), Times.Once(), "Type should be constructed once");
            //_iocContainer.Verify(i => i.Register(It.IsAny<Type>(), It.IsAny<Type>()), Times.Once(), "Type should only be registered once");
        }

        [Test]
        public void Invoke_Action_HanlderNotRegisteredFoundHandlersAndOrderAndConstructAndRegister()
        {
        }

        [Test]
        public void Invoke_Action_HanlderNotRegisteredHandlerFindOnlyAttemptedOnce()
        {
        }
    }
}