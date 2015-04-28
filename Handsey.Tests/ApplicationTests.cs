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
        }

        [Test]
        public void Initialise_NoParams_ConfigurationNotSetSoExceptionThrown()
        {
            Assert.That(() => _application.Initialise(null), Throws.Exception.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Initialise_NoParams_IocContainerOnConfigurationNotSetSoExceptionThrown()
        {
            _applicationConfiguration.SetupGet(a => a.IocConatainer).Returns(null as IIocContainer);

            Assert.That(() => _application.Initialise(_applicationConfiguration.Object), Throws.Exception.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Initialise_NoParams_AssemblyPrefixOrBaseTypeNotBeenSetSoExceptionThrown()
        {
            _applicationConfiguration.SetupGet(a => a.BaseType).Returns(null as Type);
            _applicationConfiguration.SetupGet(a => a.AssemblyNamePrefixes).Returns(null as string[]);

            Assert.That(() => _application.Initialise(_applicationConfiguration.Object), Throws.Exception.TypeOf<ArgumentException>());

            _applicationConfiguration.SetupGet(a => a.BaseType).Returns(typeof(string));
            _applicationConfiguration.SetupGet(a => a.AssemblyNamePrefixes).Returns(null as string[]);

            Assert.That(() => _application.Initialise(_applicationConfiguration.Object), Throws.Exception.TypeOf<ArgumentException>());

            _applicationConfiguration.SetupGet(a => a.BaseType).Returns(null as Type);
            _applicationConfiguration.SetupGet(a => a.AssemblyNamePrefixes).Returns(new string[1] { "test" });

            Assert.That(() => _application.Initialise(_applicationConfiguration.Object), Throws.Exception.TypeOf<ArgumentException>());
        }

        [Test]
        public void Initialise_NoParams_ApplicationInitialisedCorrectly()
        {
            _application.Initialise(_applicationConfiguration.Object);

            _handlerFactory.Setup(h => h.Create(It.IsAny<Type>(), It.IsAny<Type[]>()))
                .Returns(new List<HandlerInfo>() { new HandlerInfo() });

            _assemblyWalker.Verify(a => a.ListAllTypes(It.IsAny<Type>(), It.IsAny<string[]>()), Times.Once());
            _handlerFactory.Verify(h => h.Create(It.IsAny<Type>(), It.IsAny<Type[]>()), Times.Once());
            Assert.That(_application.ApplicationHandlers, Is.Not.Null);
        }

        [Test]
        public void Initialise_NoParams_InitialiseCanOnlyBeCalledOnce()
        {
            _application.Initialise(_applicationConfiguration.Object);

            Assert.That(() => _application.Initialise(_applicationConfiguration.Object), Throws.Exception.TypeOf<InvalidOperationException>());
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

            _application.Initialise(_applicationConfiguration.Object);
            _application.Invoke<Mock<IHandler>>(trigger.Object);

            trigger.Verify(t => t(It.IsAny<Mock<IHandler>>()), Times.Exactly(2));
        }

        [Test]
        public void Invoke_Action_HandlerNotRegisteredCreateHandlerInfoReturnsNullSoThrowRequestedHandlerNotValidException()
        {
            Mock<Action<Mock<IHandler>>> trigger = new Mock<Action<Mock<IHandler>>>();

            _handlerFactory.Setup(h => h.Create(It.IsAny<Type>(), It.IsAny<Type>()))
                .Returns<HandlerInfo>(null);

            _application.Initialise(_applicationConfiguration.Object);

            Assert.That(() => _application.Invoke<Mock<IHandler>>(trigger.Object), Throws.Exception.TypeOf<RequestedHandlerNotValidException>());
            trigger.Verify(t => t(It.IsAny<Mock<IHandler>>()), Times.Never);
            _handlerFactory.Verify(h => h.Create(It.IsAny<Type>(), It.Is<Type>(t => t == typeof(Mock<IHandler>))), Times.Once(), "All call create handler once");
            _applicationHandlers.Verify(a => a.Find(It.IsAny<HandlerInfo>(), It.IsAny<IHandlerSearch>()), Times.Never());
        }

        [Test]
        public void Invoke_Action_HandlerNotRegisteredSoFindHandlerAndReturnListAsNull()
        {
            Mock<Action<Mock<IHandler>>> trigger = new Mock<Action<Mock<IHandler>>>();

            _handlerFactory.Setup(h => h.Create(It.IsAny<Type>(), It.IsAny<Type>()))
                .Returns(() => new HandlerInfo());

            _applicationHandlers.Setup(a => a.Find(It.IsAny<HandlerInfo>(), It.IsAny<IHandlerSearch>()))
                .Returns<IEnumerable<HandlerInfo>>(null);

            _application.Initialise(_applicationConfiguration.Object);

            Assert.That(() => _application.Invoke<Mock<IHandler>>(trigger.Object), Throws.Exception.TypeOf<HandlerNotFoundException>());
            trigger.Verify(t => t(It.IsAny<Mock<IHandler>>()), Times.Never);
            _handlerFactory.Verify(h => h.Create(It.IsAny<Type>(), It.Is<Type>(t => t == typeof(Mock<IHandler>))), Times.Once(), "All call create handler once");
            _applicationHandlers.Verify(a => a.Find(It.IsAny<HandlerInfo>(), It.IsAny<IHandlerSearch>()), Times.Once(), "Should only try to find handlers once");
        }

        [Test]
        public void Invoke_Action_HandlerNotRegisteredSoFindHandlerAndReturnListAsEmpty()
        {
            Mock<Action<Mock<IHandler>>> trigger = new Mock<Action<Mock<IHandler>>>();

            _handlerFactory.Setup(h => h.Create(It.IsAny<Type>(), It.IsAny<Type>()))
                .Returns(() => new HandlerInfo());

            _applicationHandlers.Setup(a => a.Find(It.IsAny<HandlerInfo>(), It.IsAny<IHandlerSearch>()))
                .Returns(Enumerable.Empty<HandlerInfo>());

            _application.Initialise(_applicationConfiguration.Object);

            Assert.That(() => _application.Invoke<Mock<IHandler>>(trigger.Object), Throws.Exception.TypeOf<HandlerNotFoundException>());
            trigger.Verify(t => t(It.IsAny<Mock<IHandler>>()), Times.Never);
            _handlerFactory.Verify(h => h.Create(It.IsAny<Type>(), It.Is<Type>(t => t == typeof(Mock<IHandler>))), Times.Once(), "All call create handler once");
            _applicationHandlers.Verify(a => a.Find(It.IsAny<HandlerInfo>(), It.IsAny<IHandlerSearch>()), Times.Once(), "Should only try to find handlers once");
        }

        [Test]
        public void Invoke_Action_HandlerNotRegisteredFoundOneHandlerButTypeConstructorReturnsNullSoHandlerCannotBeConstructedExceptionThrown()
        {
            Mock<Action<Mock<IHandler>>> trigger = new Mock<Action<Mock<IHandler>>>();

            _handlerFactory.Setup(h => h.Create(It.IsAny<Type>(), It.IsAny<Type>()))
                .Returns(() => new HandlerInfo() { Type = typeof(Mock<IHandler>) });

            _applicationHandlers.Setup(a => a.Find(It.IsAny<HandlerInfo>(), It.IsAny<IHandlerSearch>()))
                .Returns(new List<HandlerInfo>() { new HandlerInfo() });

            _typeConstructor.Setup(t => t.Create(It.IsAny<HandlerInfo>(), It.IsAny<IList<HandlerInfo>>()))
                .Returns<IList<Type>>(null);

            _application.Initialise(_applicationConfiguration.Object);

            Assert.That(() => _application.Invoke<Mock<IHandler>>(trigger.Object), Throws.Exception.TypeOf<HandlerCannotBeConstructedException>());
            trigger.Verify(t => t(It.IsAny<Mock<IHandler>>()), Times.Never);
            _handlerFactory.Verify(h => h.Create(It.IsAny<Type>(), It.Is<Type>(t => t == typeof(Mock<IHandler>))), Times.Once(), "All call create handler once");
            _applicationHandlers.Verify(a => a.Find(It.IsAny<HandlerInfo>(), It.IsAny<IHandlerSearch>()), Times.Once(), "Should only try to find handlers once");
            _handlerSort.Verify(h => h.Sort(It.IsAny<IEnumerable<HandlerInfo>>()), Times.Never(), "Only one handler so doesn't need to be sorted");
            _typeConstructor.Verify(t => t.Create(It.IsAny<HandlerInfo>(), It.IsAny<IList<HandlerInfo>>()), Times.Once(), "Type should be constructed once");
        }

        [Test]
        public void Invoke_Action_HandlerNotRegisteredFoundOneHandlerButTypeConstructorReturnsAnEmptyListSoHandlerCannotBeConstructedExceptionThrown()
        {
            Mock<Action<Mock<IHandler>>> trigger = new Mock<Action<Mock<IHandler>>>();

            _handlerFactory.Setup(h => h.Create(It.IsAny<Type>(), It.IsAny<Type>()))
                .Returns(() => new HandlerInfo() { Type = typeof(Mock<IHandler>) });

            _applicationHandlers.Setup(a => a.Find(It.IsAny<HandlerInfo>(), It.IsAny<IHandlerSearch>()))
                .Returns(new List<HandlerInfo>() { new HandlerInfo() });

            _typeConstructor.Setup(t => t.Create(It.IsAny<HandlerInfo>(), It.IsAny<IList<HandlerInfo>>()))
                .Returns(new List<Type>());

            _application.Initialise(_applicationConfiguration.Object);

            Assert.That(() => _application.Invoke<Mock<IHandler>>(trigger.Object), Throws.Exception.TypeOf<HandlerCannotBeConstructedException>());
            trigger.Verify(t => t(It.IsAny<Mock<IHandler>>()), Times.Never);
            _handlerFactory.Verify(h => h.Create(It.IsAny<Type>(), It.Is<Type>(t => t == typeof(Mock<IHandler>))), Times.Once(), "All call create handler once");
            _applicationHandlers.Verify(a => a.Find(It.IsAny<HandlerInfo>(), It.IsAny<IHandlerSearch>()), Times.Once(), "Should only try to find handlers once");
            _handlerSort.Verify(h => h.Sort(It.IsAny<IEnumerable<HandlerInfo>>()), Times.Never(), "Only one handler so doesn't need to be sorted");
            _typeConstructor.Verify(t => t.Create(It.IsAny<HandlerInfo>(), It.IsAny<IList<HandlerInfo>>()), Times.Once(), "Type should be constructed once");
        }

        [Test]
        public void Invoke_Action_HandlerNotRegisteredFoundOneHandlerAndConstructAndNotRegisterBecauseAnotherThreadHas()
        {
            Mock<Action<Mock<IHandler>>> trigger = new Mock<Action<Mock<IHandler>>>();
            int resolveAllCount = 0;

            _handlerFactory.Setup(h => h.Create(It.IsAny<Type>(), It.IsAny<Type>()))
                .Returns(() => new HandlerInfo() { Type = typeof(Mock<IHandler>) });

            _applicationHandlers.Setup(a => a.Find(It.IsAny<HandlerInfo>(), It.IsAny<IHandlerSearch>()))
                .Returns(new List<HandlerInfo>() { new HandlerInfo() });

            _typeConstructor.Setup(t => t.Create(It.IsAny<HandlerInfo>(), It.IsAny<IList<HandlerInfo>>()))
                .Returns(new List<Type>() { typeof(IHandler) });

            _iocContainer.Setup(i => i.ResolveAll<Mock<IHandler>>())
                .Callback(() => resolveAllCount++)
                .Returns(() =>
                {
                    if (resolveAllCount == 2)
                        return new Mock<IHandler>[1] { new Mock<IHandler>() };
                    return new Mock<IHandler>[0];
                });

            _application.Initialise(_applicationConfiguration.Object);
            _application.Invoke<Mock<IHandler>>(trigger.Object);

            _handlerFactory.Verify(h => h.Create(It.IsAny<Type>(), It.Is<Type>(t => t == typeof(Mock<IHandler>))), Times.Once(), "All call create handler once");
            _applicationHandlers.Verify(a => a.Find(It.IsAny<HandlerInfo>(), It.IsAny<IHandlerSearch>()), Times.Once(), "Should only try to find handlers once");
            _handlerSort.Verify(h => h.Sort(It.IsAny<IEnumerable<HandlerInfo>>()), Times.Never(), "Only one handler so doesn't need to be sorted");
            _typeConstructor.Verify(t => t.Create(It.IsAny<HandlerInfo>(), It.IsAny<IList<HandlerInfo>>()), Times.Once(), "Type should be constructed once");
            _iocContainer.Verify(i => i.Register(It.IsAny<Type>(), It.IsAny<Type>()), Times.Never(), "TYpe should have been registered");
            _iocContainer.Verify(i => i.ResolveAll<Mock<IHandler>>(), Times.Exactly(2), "TYpe should have been registered");
            trigger.Verify(t => t(It.IsAny<Mock<IHandler>>()), Times.Once);
        }

        [Test]
        public void Invoke_Action_HandlerNotRegisteredFoundOneHandlerAndConstructAndRegister()
        {
            Mock<Action<Mock<IHandler>>> trigger = new Mock<Action<Mock<IHandler>>>();
            int resolveAllCount = 0;

            _handlerFactory.Setup(h => h.Create(It.IsAny<Type>(), It.IsAny<Type>()))
                .Returns(() => new HandlerInfo() { Type = typeof(Mock<IHandler>) });

            _applicationHandlers.Setup(a => a.Find(It.IsAny<HandlerInfo>(), It.IsAny<IHandlerSearch>()))
                .Returns(new List<HandlerInfo>() { new HandlerInfo() });

            _typeConstructor.Setup(t => t.Create(It.IsAny<HandlerInfo>(), It.IsAny<IList<HandlerInfo>>()))
                .Returns(new List<Type>() { typeof(IHandler) });

            _iocContainer.Setup(i => i.ResolveAll<Mock<IHandler>>())
                .Callback(() => resolveAllCount++)
                .Returns(() =>
                {
                    if (resolveAllCount == 3)
                        return new Mock<IHandler>[1] { new Mock<IHandler>() };
                    return new Mock<IHandler>[0];
                });

            _application.Initialise(_applicationConfiguration.Object);
            _application.Invoke<Mock<IHandler>>(trigger.Object);

            _handlerFactory.Verify(h => h.Create(It.IsAny<Type>(), It.Is<Type>(t => t == typeof(Mock<IHandler>))), Times.Once(), "All call create handler once");
            _applicationHandlers.Verify(a => a.Find(It.IsAny<HandlerInfo>(), It.IsAny<IHandlerSearch>()), Times.Once(), "Should only try to find handlers once");
            _handlerSort.Verify(h => h.Sort(It.IsAny<IEnumerable<HandlerInfo>>()), Times.Never(), "Only one handler so doesn't need to be sorted");
            _typeConstructor.Verify(t => t.Create(It.IsAny<HandlerInfo>(), It.IsAny<IList<HandlerInfo>>()), Times.Once(), "Type should be constructed once");
            _iocContainer.Verify(i => i.Register(It.IsAny<Type>(), It.IsAny<Type>()), Times.Once(), "TYpe should have been registered");
            _iocContainer.Verify(i => i.ResolveAll<Mock<IHandler>>(), Times.Exactly(3), "TYpe should have been registered");
            trigger.Verify(t => t(It.IsAny<Mock<IHandler>>()), Times.Once);
        }

        [Test]
        public void Invoke_Action_HandlerNotRegisteredFoundMultipleHandlersAndOrderAndConstructAndRegister()
        {
            Mock<Action<Mock<IHandler>>> trigger = new Mock<Action<Mock<IHandler>>>();
            int resolveAllCount = 0;

            _handlerFactory.Setup(h => h.Create(It.IsAny<Type>(), It.IsAny<Type>()))
                .Returns(() => new HandlerInfo() { Type = typeof(Mock<IHandler>) });

            _applicationHandlers.Setup(a => a.Find(It.IsAny<HandlerInfo>(), It.IsAny<IHandlerSearch>()))
                .Returns(new List<HandlerInfo>()
                {
                    new HandlerInfo(),
                    new HandlerInfo(),
                    new HandlerInfo()
                });

            _handlerSort.Setup(h => h.Sort(It.IsAny<IList<HandlerInfo>>()))
                .Returns(new List<HandlerInfo>()
                {
                    new HandlerInfo(),
                    new HandlerInfo(),
                    new HandlerInfo()
                });

            _typeConstructor.Setup(t => t.Create(It.IsAny<HandlerInfo>(), It.IsAny<IList<HandlerInfo>>()))
                .Returns(new List<Type>()
                {
                    typeof(IHandler),
                    typeof(IHandler),
                    typeof(IHandler)
                });

            _iocContainer.Setup(i => i.ResolveAll<Mock<IHandler>>())
                .Callback(() => resolveAllCount++)
                .Returns(() =>
                {
                    if (resolveAllCount == 3)
                        return new Mock<IHandler>[3] { new Mock<IHandler>(), new Mock<IHandler>(), new Mock<IHandler>() };
                    return new Mock<IHandler>[0];
                });

            _application.Initialise(_applicationConfiguration.Object);
            _application.Invoke<Mock<IHandler>>(trigger.Object);

            _handlerFactory.Verify(h => h.Create(It.IsAny<Type>(), It.Is<Type>(t => t == typeof(Mock<IHandler>))), Times.Once(), "All call create handler once");
            _applicationHandlers.Verify(a => a.Find(It.IsAny<HandlerInfo>(), It.IsAny<IHandlerSearch>()), Times.Once(), "Should only try to find handlers once");
            _handlerSort.Verify(h => h.Sort(It.IsAny<IEnumerable<HandlerInfo>>()), Times.Once(), "Multiple handlers so requires sort");
            _typeConstructor.Verify(t => t.Create(It.IsAny<HandlerInfo>(), It.IsAny<IList<HandlerInfo>>()), Times.Once(), "Type should be constructed once");
            _iocContainer.Verify(i => i.Register(It.IsAny<Type>(), It.IsAny<Type>()), Times.Exactly(3), "TYpe should have been registered");
            _iocContainer.Verify(i => i.ResolveAll<Mock<IHandler>>(), Times.Exactly(3), "TYpe should have been registered");
            trigger.Verify(t => t(It.IsAny<Mock<IHandler>>()), Times.Exactly(3));
        }
    }
}