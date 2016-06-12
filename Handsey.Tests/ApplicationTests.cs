using Handsey.Handlers;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Handsey.Tests
{
    [TestFixture]
    public class ApplicationTests
    {
        private Mock<IHandlerFactory> _handlerFactory;
        private Mock<IHandlerSearch> _handlerSearch;
        private Mock<IHandlersSort> _handlerSort;
        private Mock<ITypeConstructor> _typeConstructor;
        private Mock<IApplicationConfiguration> _applicationConfiguration;
        private Mock<IIocContainer> _iocContainer;
        private Mock<IApplicationHandlers> _applicationHandlers;
        private IApplicaton _application;

        [SetUp]
        public void Setup()
        {
            _handlerFactory = new Mock<IHandlerFactory>();
            _handlerSearch = new Mock<IHandlerSearch>();
            _handlerSort = new Mock<IHandlersSort>();
            _typeConstructor = new Mock<ITypeConstructor>();
            _applicationConfiguration = new Mock<IApplicationConfiguration>();
            _iocContainer = new Mock<IIocContainer>();
            _applicationHandlers = new Mock<IApplicationHandlers>();

            _application = new Application(_handlerFactory.Object
                , _handlerSearch.Object
                , _handlerSort.Object
                , _typeConstructor.Object
                , _iocContainer.Object
                , _applicationHandlers.Object
                , _applicationConfiguration.Object);

            _applicationConfiguration.SetupAllProperties();
            _applicationConfiguration.SetupGet(a => a.BaseType).Returns(typeof(object));
            _applicationConfiguration.SetupGet(a => a.DynamicHandlerRegistration).Returns(true);
        }

        [Test]
        public void Invoke_Action_DynamicHandlerRegistrationFalseHandlersNotRegisteredSoRequestedHandlerNotRegsiteredExceptionThrown()
        {
            Mock<Action<Mock<IHandler>>> trigger = new Mock<Action<Mock<IHandler>>>();

            _applicationConfiguration.SetupGet(a => a.DynamicHandlerRegistration).Returns(false);

            _iocContainer.Setup(i => i.ResolveAll<Mock<IHandler>>()).
                Returns(new Mock<IHandler>[0]);

            Assert.That(() => _application.Invoke<Mock<IHandler>>(trigger.Object), Throws.Exception.TypeOf<RequestedHandlerNotRegsiteredException>());
        }

        [Test]
        public async void InvokeAync_Func_DynamicHandlerRegistrationFalseHandlersNotRegisteredSoRequestedHandlerNotRegsiteredExceptionThrown()
        {
            Mock<Func<Mock<IHandler>, Task>> trigger = new Mock<Func<Mock<IHandler>, Task>>();

            _applicationConfiguration.SetupGet(a => a.DynamicHandlerRegistration).Returns(false);

            _iocContainer.Setup(i => i.ResolveAll<Mock<IHandler>>()).
                Returns(new Mock<IHandler>[0]);

            Assert.That(async () => await _application.InvokeAsync<Mock<IHandler>>(trigger.Object), Throws.Exception.TypeOf<RequestedHandlerNotRegsiteredException>());
        }

        [Test]
        public void Invoke_Action_DynamicHandlerRegistrationFalseHandlersRegisteredSoTriggered()
        {
            Mock<Action<Mock<IHandler>>> trigger = new Mock<Action<Mock<IHandler>>>();

            _applicationConfiguration.SetupGet(a => a.DynamicHandlerRegistration).Returns(false);

            _iocContainer.Setup(i => i.ResolveAll<Mock<IHandler>>()).
                Returns(new Mock<IHandler>[1] { new Mock<IHandler>() });

            _application.Invoke<Mock<IHandler>>(trigger.Object);

            _handlerFactory.Verify(h => h.Create(It.Is<Type>(t => t == typeof(Mock<IHandler>))), Times.Never(), "HandlerFactory.Create should not be called");
            _applicationHandlers.Verify(a => a.Find(It.IsAny<HandlerInfo>(), It.IsAny<IHandlerSearch>()), Times.Never(), "ApplicationHanlders.Find should not be called");
            _handlerSort.Verify(h => h.Sort(It.IsAny<IEnumerable<HandlerInfo>>()), Times.Never(), "HandlerSort.Sort should not be called");
            _typeConstructor.Verify(t => t.Create(It.IsAny<HandlerInfo>(), It.IsAny<IList<HandlerInfo>>()), Times.Never(), "TypeConstructor.Create should not be called");
            _iocContainer.Verify(i => i.Register(It.IsAny<Type>(), It.IsAny<Type>(), It.IsAny<string>()), Times.Never(), "IocContainer.Register should not be called");

            _iocContainer.Verify(i => i.ResolveAll<Mock<IHandler>>(), Times.Once, "Resolve all should be called");
            trigger.Verify(t => t(It.IsAny<Mock<IHandler>>()), Times.Once);
        }

        [Test]
        public async void InvokeAsync_Func_DynamicHandlerRegistrationFalseHandlersRegisteredSoTriggered()
        {
            Mock<Func<Mock<IHandler>, Task>> trigger = new Mock<Func<Mock<IHandler>, Task>>();

            _applicationConfiguration.SetupGet(a => a.DynamicHandlerRegistration).Returns(false);

            _iocContainer.Setup(i => i.ResolveAll<Mock<IHandler>>()).
                Returns(new Mock<IHandler>[1] { new Mock<IHandler>() });

            await _application.InvokeAsync(trigger.Object);

            _handlerFactory.Verify(h => h.Create(It.Is<Type>(t => t == typeof(Mock<IHandler>))), Times.Never(), "HandlerFactory.Create should not be called");
            _applicationHandlers.Verify(a => a.Find(It.IsAny<HandlerInfo>(), It.IsAny<IHandlerSearch>()), Times.Never(), "ApplicationHanlders.Find should not be called");
            _handlerSort.Verify(h => h.Sort(It.IsAny<IEnumerable<HandlerInfo>>()), Times.Never(), "HandlerSort.Sort should not be called");
            _typeConstructor.Verify(t => t.Create(It.IsAny<HandlerInfo>(), It.IsAny<IList<HandlerInfo>>()), Times.Never(), "TypeConstructor.Create should not be called");
            _iocContainer.Verify(i => i.Register(It.IsAny<Type>(), It.IsAny<Type>(), It.IsAny<string>()), Times.Never(), "IocContainer.Register should not be called");

            _iocContainer.Verify(i => i.ResolveAll<Mock<IHandler>>(), Times.Once, "Resolve all should be called");
            trigger.Verify(t => t(It.IsAny<Mock<IHandler>>()), Times.Once);
        }

        [Test]
        public void Invoke_Action_HandlerResolvesSoTriggerCalledOnEachHandler()
        {
            Mock<Action<Mock<IHandler>>> trigger = new Mock<Action<Mock<IHandler>>>();

            _iocContainer.Setup(i => i.ResolveAll<Mock<IHandler>>()).
                Returns(new Mock<IHandler>[2] { new Mock<IHandler>(), new Mock<IHandler>() });

            _application.Invoke<Mock<IHandler>>(trigger.Object);

            trigger.Verify(t => t(It.IsAny<Mock<IHandler>>()), Times.Exactly(2));
        }

        [Test]
        public async void InvokeAsync_Func_HandlerResolvesSoTriggerCalledOnEachHandler()
        {
            Mock<Func<Mock<IHandler>, Task>> trigger = new Mock<Func<Mock<IHandler>, Task>>();

            _iocContainer.Setup(i => i.ResolveAll<Mock<IHandler>>()).
                Returns(new Mock<IHandler>[2] { new Mock<IHandler>(), new Mock<IHandler>() });

            await _application.InvokeAsync(trigger.Object);

            trigger.Verify(t => t(It.IsAny<Mock<IHandler>>()), Times.Exactly(2));
        }

        [Test]
        public void Invoke_Action_HandlerNotRegisteredCreateHandlerInfoReturnsNullSoThrowRequestedHandlerNotValidException()
        {
            Mock<Action<Mock<IHandler>>> trigger = new Mock<Action<Mock<IHandler>>>();

            _handlerFactory.Setup(h => h.Create(It.IsAny<Type>()))
                .Returns<HandlerInfo>(null);

            Assert.That(() => _application.Invoke<Mock<IHandler>>(trigger.Object), Throws.Exception.TypeOf<RequestedHandlerNotValidException>());
            trigger.Verify(t => t(It.IsAny<Mock<IHandler>>()), Times.Never);
            _handlerFactory.Verify(h => h.Create(It.Is<Type>(t => t == typeof(Mock<IHandler>))), Times.Once(), "All call create handler once");
            _applicationHandlers.Verify(a => a.Find(It.IsAny<HandlerInfo>(), It.IsAny<IHandlerSearch>()), Times.Never());
        }

        [Test]
        public async void InvokeAsync_Func_HandlerNotRegisteredCreateHandlerInfoReturnsNullSoThrowRequestedHandlerNotValidException()
        {
            Mock<Func<Mock<IHandler>, Task>> trigger = new Mock<Func<Mock<IHandler>, Task>>();

            _handlerFactory.Setup(h => h.Create(It.IsAny<Type>()))
                .Returns<HandlerInfo>(null);

            Assert.That(async () => await _application.InvokeAsync(trigger.Object), Throws.Exception.TypeOf<RequestedHandlerNotValidException>());
            trigger.Verify(t => t(It.IsAny<Mock<IHandler>>()), Times.Never);
            _handlerFactory.Verify(h => h.Create(It.Is<Type>(t => t == typeof(Mock<IHandler>))), Times.Once(), "All call create handler once");
            _applicationHandlers.Verify(a => a.Find(It.IsAny<HandlerInfo>(), It.IsAny<IHandlerSearch>()), Times.Never());
        }

        [Test]
        public void Invoke_Action_HandlerNotRegisteredSoFindHandlerAndReturnListAsNull()
        {
            Mock<Action<Mock<IHandler>>> trigger = new Mock<Action<Mock<IHandler>>>();

            _handlerFactory.Setup(h => h.Create(It.IsAny<Type>()))
                .Returns(() => new HandlerInfo());

            _applicationHandlers.Setup(a => a.Find(It.IsAny<HandlerInfo>(), It.IsAny<IHandlerSearch>()))
                .Returns<IEnumerable<HandlerInfo>>(null);

            Assert.That(() => _application.Invoke<Mock<IHandler>>(trigger.Object), Throws.Exception.TypeOf<HandlerNotFoundException>());
            trigger.Verify(t => t(It.IsAny<Mock<IHandler>>()), Times.Never);
            _handlerFactory.Verify(h => h.Create(It.Is<Type>(t => t == typeof(Mock<IHandler>))), Times.Once(), "All call create handler once");
            _applicationHandlers.Verify(a => a.Find(It.IsAny<HandlerInfo>(), It.IsAny<IHandlerSearch>()), Times.Once(), "Should only try to find handlers once");
        }

        [Test]
        public async void InvokeAsync_Func_HandlerNotRegisteredSoFindHandlerAndReturnListAsNull()
        {
            Mock<Func<Mock<IHandler>, Task>> trigger = new Mock<Func<Mock<IHandler>, Task>>();

            _handlerFactory.Setup(h => h.Create(It.IsAny<Type>()))
                .Returns(() => new HandlerInfo());

            _applicationHandlers.Setup(a => a.Find(It.IsAny<HandlerInfo>(), It.IsAny<IHandlerSearch>()))
                .Returns<IEnumerable<HandlerInfo>>(null);

            Assert.That(async () => await _application.InvokeAsync(trigger.Object), Throws.Exception.TypeOf<HandlerNotFoundException>());
            trigger.Verify(t => t(It.IsAny<Mock<IHandler>>()), Times.Never);
            _handlerFactory.Verify(h => h.Create(It.Is<Type>(t => t == typeof(Mock<IHandler>))), Times.Once(), "All call create handler once");
            _applicationHandlers.Verify(a => a.Find(It.IsAny<HandlerInfo>(), It.IsAny<IHandlerSearch>()), Times.Once(), "Should only try to find handlers once");
        }

        [Test]
        public void Invoke_Action_HandlerNotRegisteredSoFindHandlerAndReturnListAsEmpty()
        {
            Mock<Action<Mock<IHandler>>> trigger = new Mock<Action<Mock<IHandler>>>();

            _handlerFactory.Setup(h => h.Create(It.IsAny<Type>()))
                .Returns(() => new HandlerInfo());

            _applicationHandlers.Setup(a => a.Find(It.IsAny<HandlerInfo>(), It.IsAny<IHandlerSearch>()))
                .Returns(Enumerable.Empty<HandlerInfo>());

            Assert.That(() => _application.Invoke<Mock<IHandler>>(trigger.Object), Throws.Exception.TypeOf<HandlerNotFoundException>());
            trigger.Verify(t => t(It.IsAny<Mock<IHandler>>()), Times.Never);
            _handlerFactory.Verify(h => h.Create(It.Is<Type>(t => t == typeof(Mock<IHandler>))), Times.Once(), "All call create handler once");
            _applicationHandlers.Verify(a => a.Find(It.IsAny<HandlerInfo>(), It.IsAny<IHandlerSearch>()), Times.Once(), "Should only try to find handlers once");
        }

        [Test]
        public async void InvokeAsync_Func_HandlerNotRegisteredSoFindHandlerAndReturnListAsEmpty()
        {
            Mock<Func<Mock<IHandler>, Task>> trigger = new Mock<Func<Mock<IHandler>, Task>>();

            _handlerFactory.Setup(h => h.Create(It.IsAny<Type>()))
                .Returns(() => new HandlerInfo());

            _applicationHandlers.Setup(a => a.Find(It.IsAny<HandlerInfo>(), It.IsAny<IHandlerSearch>()))
                .Returns(Enumerable.Empty<HandlerInfo>());

            Assert.That(async () => await _application.InvokeAsync<Mock<IHandler>>(trigger.Object), Throws.Exception.TypeOf<HandlerNotFoundException>());
            trigger.Verify(t => t(It.IsAny<Mock<IHandler>>()), Times.Never);
            _handlerFactory.Verify(h => h.Create(It.Is<Type>(t => t == typeof(Mock<IHandler>))), Times.Once(), "All call create handler once");
            _applicationHandlers.Verify(a => a.Find(It.IsAny<HandlerInfo>(), It.IsAny<IHandlerSearch>()), Times.Once(), "Should only try to find handlers once");
        }

        [Test]
        public void Invoke_Action_HandlerNotRegisteredFoundOneHandlerButTypeConstructorReturnsNullSoHandlerCannotBeConstructedExceptionThrown()
        {
            Mock<Action<Mock<IHandler>>> trigger = new Mock<Action<Mock<IHandler>>>();

            _handlerFactory.Setup(h => h.Create(It.IsAny<Type>()))
                .Returns(() => new HandlerInfo() { Type = typeof(Mock<IHandler>) });

            _applicationHandlers.Setup(a => a.Find(It.IsAny<HandlerInfo>(), It.IsAny<IHandlerSearch>()))
                .Returns(new List<HandlerInfo>() { new HandlerInfo() });

            _typeConstructor.Setup(t => t.Create(It.IsAny<HandlerInfo>(), It.IsAny<IList<HandlerInfo>>()))
                .Returns<IList<Type>>(null);

            Assert.That(() => _application.Invoke<Mock<IHandler>>(trigger.Object), Throws.Exception.TypeOf<HandlerCannotBeConstructedException>());
            trigger.Verify(t => t(It.IsAny<Mock<IHandler>>()), Times.Never);
            _handlerFactory.Verify(h => h.Create(It.Is<Type>(t => t == typeof(Mock<IHandler>))), Times.Once(), "All call create handler once");
            _applicationHandlers.Verify(a => a.Find(It.IsAny<HandlerInfo>(), It.IsAny<IHandlerSearch>()), Times.Once(), "Should only try to find handlers once");
            _handlerSort.Verify(h => h.Sort(It.IsAny<IEnumerable<HandlerInfo>>()), Times.Never(), "Only one handler so doesn't need to be sorted");
            _typeConstructor.Verify(t => t.Create(It.IsAny<HandlerInfo>(), It.IsAny<IList<HandlerInfo>>()), Times.Once(), "Type should be constructed once");
        }

        [Test]
        public void InvokeAsync_Func_HandlerNotRegisteredFoundOneHandlerButTypeConstructorReturnsNullSoHandlerCannotBeConstructedExceptionThrown()
        {
            Mock<Func<Mock<IHandler>, Task>> trigger = new Mock<Func<Mock<IHandler>, Task>>();

            _handlerFactory.Setup(h => h.Create(It.IsAny<Type>()))
                .Returns(() => new HandlerInfo() { Type = typeof(Mock<IHandler>) });

            _applicationHandlers.Setup(a => a.Find(It.IsAny<HandlerInfo>(), It.IsAny<IHandlerSearch>()))
                .Returns(new List<HandlerInfo>() { new HandlerInfo() });

            _typeConstructor.Setup(t => t.Create(It.IsAny<HandlerInfo>(), It.IsAny<IList<HandlerInfo>>()))
                .Returns<IList<Type>>(null);

            Assert.That(async () => await _application.InvokeAsync(trigger.Object), Throws.Exception.TypeOf<HandlerCannotBeConstructedException>());
            trigger.Verify(t => t(It.IsAny<Mock<IHandler>>()), Times.Never);
            _handlerFactory.Verify(h => h.Create(It.Is<Type>(t => t == typeof(Mock<IHandler>))), Times.Once(), "All call create handler once");
            _applicationHandlers.Verify(a => a.Find(It.IsAny<HandlerInfo>(), It.IsAny<IHandlerSearch>()), Times.Once(), "Should only try to find handlers once");
            _handlerSort.Verify(h => h.Sort(It.IsAny<IEnumerable<HandlerInfo>>()), Times.Never(), "Only one handler so doesn't need to be sorted");
            _typeConstructor.Verify(t => t.Create(It.IsAny<HandlerInfo>(), It.IsAny<IList<HandlerInfo>>()), Times.Once(), "Type should be constructed once");
        }

        [Test]
        public void Invoke_Action_HandlerNotRegisteredFoundOneHandlerButTypeConstructorReturnsAnEmptyListSoHandlerCannotBeConstructedExceptionThrown()
        {
            Mock<Action<Mock<IHandler>>> trigger = new Mock<Action<Mock<IHandler>>>();

            _handlerFactory.Setup(h => h.Create(It.IsAny<Type>()))
                .Returns(() => new HandlerInfo() { Type = typeof(Mock<IHandler>) });

            _applicationHandlers.Setup(a => a.Find(It.IsAny<HandlerInfo>(), It.IsAny<IHandlerSearch>()))
                .Returns(new List<HandlerInfo>() { new HandlerInfo() });

            _typeConstructor.Setup(t => t.Create(It.IsAny<HandlerInfo>(), It.IsAny<IList<HandlerInfo>>()))
                .Returns(new List<Type>());

            Assert.That(() => _application.Invoke<Mock<IHandler>>(trigger.Object), Throws.Exception.TypeOf<HandlerCannotBeConstructedException>());
            trigger.Verify(t => t(It.IsAny<Mock<IHandler>>()), Times.Never);
            _handlerFactory.Verify(h => h.Create(It.Is<Type>(t => t == typeof(Mock<IHandler>))), Times.Once(), "All call create handler once");
            _applicationHandlers.Verify(a => a.Find(It.IsAny<HandlerInfo>(), It.IsAny<IHandlerSearch>()), Times.Once(), "Should only try to find handlers once");
            _handlerSort.Verify(h => h.Sort(It.IsAny<IEnumerable<HandlerInfo>>()), Times.Never(), "Only one handler so doesn't need to be sorted");
            _typeConstructor.Verify(t => t.Create(It.IsAny<HandlerInfo>(), It.IsAny<IList<HandlerInfo>>()), Times.Once(), "Type should be constructed once");
        }

        [Test]
        public async void Invoke_Func_HandlerNotRegisteredFoundOneHandlerButTypeConstructorReturnsAnEmptyListSoHandlerCannotBeConstructedExceptionThrown()
        {
            Mock<Func<Mock<IHandler>, Task>> trigger = new Mock<Func<Mock<IHandler>, Task>>();

            _handlerFactory.Setup(h => h.Create(It.IsAny<Type>()))
                .Returns(() => new HandlerInfo() { Type = typeof(Mock<IHandler>) });

            _applicationHandlers.Setup(a => a.Find(It.IsAny<HandlerInfo>(), It.IsAny<IHandlerSearch>()))
                .Returns(new List<HandlerInfo>() { new HandlerInfo() });

            _typeConstructor.Setup(t => t.Create(It.IsAny<HandlerInfo>(), It.IsAny<IList<HandlerInfo>>()))
                .Returns(new List<Type>());

            Assert.That(async () => await _application.InvokeAsync(trigger.Object), Throws.Exception.TypeOf<HandlerCannotBeConstructedException>());
            trigger.Verify(t => t(It.IsAny<Mock<IHandler>>()), Times.Never);
            _handlerFactory.Verify(h => h.Create(It.Is<Type>(t => t == typeof(Mock<IHandler>))), Times.Once(), "All call create handler once");
            _applicationHandlers.Verify(a => a.Find(It.IsAny<HandlerInfo>(), It.IsAny<IHandlerSearch>()), Times.Once(), "Should only try to find handlers once");
            _handlerSort.Verify(h => h.Sort(It.IsAny<IEnumerable<HandlerInfo>>()), Times.Never(), "Only one handler so doesn't need to be sorted");
            _typeConstructor.Verify(t => t.Create(It.IsAny<HandlerInfo>(), It.IsAny<IList<HandlerInfo>>()), Times.Once(), "Type should be constructed once");
        }

        [Test]
        public void Invoke_Action_HandlerNotRegisteredFoundOneHandlerAndConstructAndNotRegisterBecauseAnotherThreadHas()
        {
            Mock<Action<Mock<IHandler>>> trigger = new Mock<Action<Mock<IHandler>>>();
            int resolveAllCount = 0;

            _handlerFactory.Setup(h => h.Create(It.IsAny<Type>()))
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

            _application.Invoke<Mock<IHandler>>(trigger.Object);

            _iocContainer.Verify(i => i.Register(It.IsAny<Type>(), It.IsAny<Type>(), It.IsAny<string>()), Times.Never(), "TYpe should have been registered");
            _iocContainer.Verify(i => i.ResolveAll<Mock<IHandler>>(), Times.Exactly(2), "TYpe should have been registered");

            _handlerFactory.Verify(h => h.Create(It.Is<Type>(t => t == typeof(Mock<IHandler>))), Times.Never(), "Handler factory should not be called");
            _applicationHandlers.Verify(a => a.Find(It.IsAny<HandlerInfo>(), It.IsAny<IHandlerSearch>()), Times.Never(), "application handlers should not be looked for");
            _handlerSort.Verify(h => h.Sort(It.IsAny<IEnumerable<HandlerInfo>>()), Times.Never(), "Only one handler so doesn't need to be sorted");
            _typeConstructor.Verify(t => t.Create(It.IsAny<HandlerInfo>(), It.IsAny<IList<HandlerInfo>>()), Times.Never(), "Type should not be constructed");

            trigger.Verify(t => t(It.IsAny<Mock<IHandler>>()), Times.Once);
        }

        [Test]
        public async void InvokeAsync_Func_HandlerNotRegisteredFoundOneHandlerAndConstructAndNotRegisterBecauseAnotherThreadHas()
        {
            Mock<Func<Mock<IHandler>, Task>> trigger = new Mock<Func<Mock<IHandler>, Task>>();
            int resolveAllCount = 0;

            _handlerFactory.Setup(h => h.Create(It.IsAny<Type>()))
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

            await _application.InvokeAsync(trigger.Object);

            _iocContainer.Verify(i => i.Register(It.IsAny<Type>(), It.IsAny<Type>(), It.IsAny<string>()), Times.Never(), "TYpe should have been registered");
            _iocContainer.Verify(i => i.ResolveAll<Mock<IHandler>>(), Times.Exactly(2), "TYpe should have been registered");

            _handlerFactory.Verify(h => h.Create(It.Is<Type>(t => t == typeof(Mock<IHandler>))), Times.Never(), "Handler factory should not be called");
            _applicationHandlers.Verify(a => a.Find(It.IsAny<HandlerInfo>(), It.IsAny<IHandlerSearch>()), Times.Never(), "application handlers should not be looked for");
            _handlerSort.Verify(h => h.Sort(It.IsAny<IEnumerable<HandlerInfo>>()), Times.Never(), "Only one handler so doesn't need to be sorted");
            _typeConstructor.Verify(t => t.Create(It.IsAny<HandlerInfo>(), It.IsAny<IList<HandlerInfo>>()), Times.Never(), "Type should not be constructed");

            trigger.Verify(t => t(It.IsAny<Mock<IHandler>>()), Times.Once);
        }

        [Test]
        public void Invoke_Action_HandlerNotRegisteredFoundOneHandlerAndConstructAndRegister()
        {
            Mock<Action<Mock<IHandler>>> trigger = new Mock<Action<Mock<IHandler>>>();
            int resolveAllCount = 0;

            _handlerFactory.Setup(h => h.Create(It.IsAny<Type>()))
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

            _application.Invoke<Mock<IHandler>>(trigger.Object);

            _handlerFactory.Verify(h => h.Create(It.Is<Type>(t => t == typeof(Mock<IHandler>))), Times.Once(), "All call create handler once");
            _applicationHandlers.Verify(a => a.Find(It.IsAny<HandlerInfo>(), It.IsAny<IHandlerSearch>()), Times.Once(), "Should only try to find handlers once");
            _handlerSort.Verify(h => h.Sort(It.IsAny<IEnumerable<HandlerInfo>>()), Times.Never(), "Only one handler so doesn't need to be sorted");
            _typeConstructor.Verify(t => t.Create(It.IsAny<HandlerInfo>(), It.IsAny<IList<HandlerInfo>>()), Times.Once(), "Type should be constructed once");
            _iocContainer.Verify(i => i.Register(It.IsAny<Type>(), It.IsAny<Type>(), It.IsAny<string>()), Times.Once(), "TYpe should have been registered");
            _iocContainer.Verify(i => i.ResolveAll<Mock<IHandler>>(), Times.Exactly(3), "TYpe should have been registered");
            trigger.Verify(t => t(It.IsAny<Mock<IHandler>>()), Times.Once);
        }

        [Test]
        public async void InvokeAsync_Func_HandlerNotRegisteredFoundOneHandlerAndConstructAndRegister()
        {
            Mock<Func<Mock<IHandler>, Task>> trigger = new Mock<Func<Mock<IHandler>, Task>>();
            int resolveAllCount = 0;

            _handlerFactory.Setup(h => h.Create(It.IsAny<Type>()))
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

            await _application.InvokeAsync(trigger.Object);

            _handlerFactory.Verify(h => h.Create(It.Is<Type>(t => t == typeof(Mock<IHandler>))), Times.Once(), "All call create handler once");
            _applicationHandlers.Verify(a => a.Find(It.IsAny<HandlerInfo>(), It.IsAny<IHandlerSearch>()), Times.Once(), "Should only try to find handlers once");
            _handlerSort.Verify(h => h.Sort(It.IsAny<IEnumerable<HandlerInfo>>()), Times.Never(), "Only one handler so doesn't need to be sorted");
            _typeConstructor.Verify(t => t.Create(It.IsAny<HandlerInfo>(), It.IsAny<IList<HandlerInfo>>()), Times.Once(), "Type should be constructed once");
            _iocContainer.Verify(i => i.Register(It.IsAny<Type>(), It.IsAny<Type>(), It.IsAny<string>()), Times.Once(), "TYpe should have been registered");
            _iocContainer.Verify(i => i.ResolveAll<Mock<IHandler>>(), Times.Exactly(3), "TYpe should have been registered");
            trigger.Verify(t => t(It.IsAny<Mock<IHandler>>()), Times.Once);
        }

        [Test]
        public void Invoke_Action_HandlerNotRegisteredFoundMultipleHandlersAndOrderAndConstructAndRegister()
        {
            Mock<Action<Mock<IHandler>>> trigger = new Mock<Action<Mock<IHandler>>>();
            int resolveAllCount = 0;

            _handlerFactory.Setup(h => h.Create(It.IsAny<Type>()))
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

            _application.Invoke<Mock<IHandler>>(trigger.Object);

            _handlerFactory.Verify(h => h.Create(It.Is<Type>(t => t == typeof(Mock<IHandler>))), Times.Once(), "All call create handler once");
            _applicationHandlers.Verify(a => a.Find(It.IsAny<HandlerInfo>(), It.IsAny<IHandlerSearch>()), Times.Once(), "Should only try to find handlers once");
            _handlerSort.Verify(h => h.Sort(It.IsAny<IEnumerable<HandlerInfo>>()), Times.Once(), "Multiple handlers so requires sort");
            _typeConstructor.Verify(t => t.Create(It.IsAny<HandlerInfo>(), It.IsAny<IList<HandlerInfo>>()), Times.Once(), "Type should be constructed once");
            _iocContainer.Verify(i => i.Register(It.IsAny<Type>(), It.IsAny<Type>(), It.IsAny<string>()), Times.Exactly(3), "TYpe should have been registered");
            _iocContainer.Verify(i => i.ResolveAll<Mock<IHandler>>(), Times.Exactly(3), "TYpe should have been registered");
            trigger.Verify(t => t(It.IsAny<Mock<IHandler>>()), Times.Exactly(3));
        }

        [Test]
        public async void InvokeAsync_Func_HandlerNotRegisteredFoundMultipleHandlersAndOrderAndConstructAndRegister()
        {
            Mock<Func<Mock<IHandler>, Task>> trigger = new Mock<Func<Mock<IHandler>, Task>>();
            int resolveAllCount = 0;

            _handlerFactory.Setup(h => h.Create(It.IsAny<Type>()))
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

            await _application.InvokeAsync(trigger.Object);

            _handlerFactory.Verify(h => h.Create(It.Is<Type>(t => t == typeof(Mock<IHandler>))), Times.Once(), "All call create handler once");
            _applicationHandlers.Verify(a => a.Find(It.IsAny<HandlerInfo>(), It.IsAny<IHandlerSearch>()), Times.Once(), "Should only try to find handlers once");
            _handlerSort.Verify(h => h.Sort(It.IsAny<IEnumerable<HandlerInfo>>()), Times.Once(), "Multiple handlers so requires sort");
            _typeConstructor.Verify(t => t.Create(It.IsAny<HandlerInfo>(), It.IsAny<IList<HandlerInfo>>()), Times.Once(), "Type should be constructed once");
            _iocContainer.Verify(i => i.Register(It.IsAny<Type>(), It.IsAny<Type>(), It.IsAny<string>()), Times.Exactly(3), "TYpe should have been registered");
            _iocContainer.Verify(i => i.ResolveAll<Mock<IHandler>>(), Times.Exactly(3), "TYpe should have been registered");
            trigger.Verify(t => t(It.IsAny<Mock<IHandler>>()), Times.Exactly(3));
        }

        [Test]
        public void RegisterAll_NoParams_HandlerAvailableSoRegistered()
        {
            _handlerFactory.Setup(h => h.Create(It.IsAny<Type>()))
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

            _application.RegisterAll<Mock<IHandler>>();

            _handlerFactory.Verify(h => h.Create(It.Is<Type>(t => t == typeof(Mock<IHandler>))), Times.Once(), "All call create handler once");
            _applicationHandlers.Verify(a => a.Find(It.IsAny<HandlerInfo>(), It.IsAny<IHandlerSearch>()), Times.Once(), "Should only try to find handlers once");
            _handlerSort.Verify(h => h.Sort(It.IsAny<IEnumerable<HandlerInfo>>()), Times.Once(), "Multiple handlers so requires sort");
            _typeConstructor.Verify(t => t.Create(It.IsAny<HandlerInfo>(), It.IsAny<IList<HandlerInfo>>()), Times.Once(), "Type should be constructed once");
            _iocContainer.Verify(i => i.Register(It.IsAny<Type>(), It.IsAny<Type>(), It.IsAny<string>()), Times.Exactly(3), "TYpe should have been registered");
            _iocContainer.Verify(i => i.ResolveAll<Mock<IHandler>>(), Times.Never, "TYpe should never be resolved");
        }
    }
}