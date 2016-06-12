using Handsey.Handlers;
using Handsey.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Handsey
{
    public sealed class Application : IApplicaton
    {
        #region // Fields

        private readonly IHandlerFactory _handlerFactory;
        private readonly IHandlerSearch _handlerSearch;
        private readonly IHandlersSort _handlerSort;
        private readonly ITypeConstructor _typeConstructor;
        private readonly IApplicationHandlers _applicationHandlers;
        private readonly IIocContainer _iocContainer;
        private readonly IApplicationConfiguration _applicationConfiguration;

        #endregion // Fields

        #region // Constructors

        public Application(IHandlerFactory handlerFactory
            , IHandlerSearch handlerSearch
            , IHandlersSort handlerSort
            , ITypeConstructor typeConstructor
            , IIocContainer iocContainer
            , IApplicationHandlers applicationHandlers
            , IApplicationConfiguration applicationConfiguration)
        {
            _handlerFactory = handlerFactory;
            _handlerSearch = handlerSearch;
            _handlerSort = handlerSort;
            _typeConstructor = typeConstructor;
            _iocContainer = iocContainer;
            _applicationHandlers = applicationHandlers;
            _applicationConfiguration = applicationConfiguration;
        }

        #endregion // Constructors

        #region Methods

        /// <summary>
        /// Registers all matching handlers without invoking them. Us this method with ApplicationConfiguration.DynamicHandlerRegistration set to false
        /// </summary>
        /// <typeparam name="THandler"></typeparam>
        public void RegisterAll<THandler>()
        {
            RegisterHandlersWithWriteLock<THandler>(null);
        }

        /// <summary>
        /// Invoke with dynamic regisration (depends on application DynamicHandlerRegistration flag)
        /// </summary>
        /// <typeparam name="THandler"></typeparam>
        /// <param name="trigger"></param>
        public bool Invoke<THandler>(Action<THandler> trigger)
        {
            if (!_applicationConfiguration.DynamicHandlerRegistration)
            {
                // Try and invoke handlers
                return TryInvokeWithReadLock(ResolveHandlers<THandler>(), trigger);
            }

            // try to resolve
            if (TryInvokeWithReadLock(ResolveHandlers<THandler>(), trigger))
                return true;

            // if we are here we couldn't resolve handlers
            if (!RegisterHandlersWithWriteLock<THandler>(trigger))
                return true;

            // Construct the handlers from the IOC container now we have registered them
            return TryInvokeWithReadLock(ResolveHandlers<THandler>(), trigger);
        }

        /// <summary>
        /// Invoke handlers asynchronously with dynamic regisration (depends on application DynamicHandlerRegistration flag).
        /// Dynamic registration occurs synchronously.
        /// </summary>
        /// <typeparam name="THandler"></typeparam>
        /// <param name="trigger"></param>
        public async Task<bool> InvokeAsync<THandler>(Func<THandler, Task> trigger)
        {
            if (!_applicationConfiguration.DynamicHandlerRegistration)
            {
                return await TryInvokeWithReadLockAsync(ResolveHandlers<THandler>(), trigger);
            }

            // try to resolve
            if (await TryInvokeWithReadLockAsync(ResolveHandlers<THandler>(), trigger))
                return true;

            // if we are here we couldn't resolve handlers
            // There is no point performing an await" here because the method is thread blocking
            if (!RegisterHandlersWithWriteLock<THandler>((h) => trigger(h)))
                return true;

            // Construct the handlers from the IOC container now we have registered them
            return await TryInvokeWithReadLockAsync(ResolveHandlers<THandler>(), trigger);
        }

        /// <summary>
        /// Thread safe method to search and register for handlers.
        /// Returns:
        /// True if handlers were registered.
        /// False if handlers were invoked because they were already registered by another thread
        /// </summary>
        /// <typeparam name="THandler"></typeparam>
        /// <param name="trigger">If null is passed in double-check locking will not be performed</param>
        /// <returns>False if the handlers were invoked without registering</returns>
        private bool RegisterHandlersWithWriteLock<THandler>(Action<THandler> trigger)
        {
            Lock<THandler>.ReaderWriterLockSlim.EnterWriteLock();
            try
            {
                // This second check is a form of "Double-Check Locking" idiom
                // Make sure the handlers haven't been registered in another thread.
                // If it has been registered then invoke
                if (trigger != null && TryInvoke(ResolveHandlers<THandler>(), trigger))
                    return false;

                RegisterHandlers<THandler>();

                return true;
            }
            finally
            {
                // end write lock
                Lock<THandler>.ReaderWriterLockSlim.ExitWriteLock();
            }
        }

        private bool RegisterHandlers<THandler>()
        {
            // create handler
            HandlerInfo toSearchFor = CreateHandler<THandler>();

            // Find
            IEnumerable<HandlerInfo> handlers;
            if (!TryFindHandlers<THandler>(toSearchFor, out handlers))
                return false;

            // order
            IList<HandlerInfo> handlersList = Order(handlers.ToList());
            // construct
            IEnumerable<Type> constructedTypes = ConstructTypes(toSearchFor, handlersList);

            // register handler
            RegisterTypesAsHandler<THandler>(constructedTypes);

            return true;
        }

        private bool TryInvokeWithReadLock<THandler>(THandler[] handlers, Action<THandler> trigger)
        {
            // Enter read lock
            Lock<THandler>.ReaderWriterLockSlim.EnterReadLock();
            try
            {
                return TryInvoke<THandler>(handlers, trigger);
            }
            finally
            {
                Lock<THandler>.ReaderWriterLockSlim.ExitReadLock();
            }
        }

        private async Task<bool> TryInvokeWithReadLockAsync<THandler>(THandler[] handlers, Func<THandler, Task> trigger)
        {
            // Enter read lock
            Lock<THandler>.ReaderWriterLockSlim.EnterReadLock();
            try
            {
                return await TryInvokeAsync<THandler>(handlers, trigger);
            }
            finally
            {
                Lock<THandler>.ReaderWriterLockSlim.ExitReadLock();
            }
        }

        private THandler[] ResolveHandlers<THandler>()
        {
            return _iocContainer.ResolveAll<THandler>();
        }

        private async Task<bool> TryInvokeAsync<THandler>(THandler[] handlers, Func<THandler, Task> trigger)
        {
            bool invoked = false;

            foreach (THandler handle in handlers)
            {
                await trigger(handle);
                invoked = true;
            }

            return invoked;
        }

        private bool TryInvoke<THandler>(THandler[] handlers, Action<THandler> trigger)
        {
            bool invoked = false;

            foreach (THandler handle in handlers)
            {
                trigger(handle);
                invoked = true;
            }

            return invoked;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="THandler"></typeparam>
        /// <exception cref="RequestedHandlerNotValidException"></exception>
        /// <returns></returns>
        private HandlerInfo CreateHandler<THandler>()
        {
            HandlerInfo toSearchFor = null;

            PerformCheck.IsTrue(() => !TryCreateHandler<THandler>(_applicationConfiguration.BaseType, out toSearchFor))
                .Throw<RequestedHandlerNotValidException>(() =>
                    new RequestedHandlerNotValidException("Requested handler " + typeof(THandler).FullName + " cannot be converted to a handler.")
                    );

            return toSearchFor;
        }

        private bool TryCreateHandler<THandler>(Type baseType, out HandlerInfo handlerInfo)
        {
            handlerInfo = _handlerFactory.Create(typeof(THandler));
            return handlerInfo != null;
        }

        /// <summary>
        /// Finds handlers in the ApplicationHandlers object
        /// </summary>
        /// <param name="toSearchFor"></param>
        /// <returns></returns>
        private bool TryFindHandlers<THandler>(HandlerInfo toSearchFor, out IEnumerable<HandlerInfo> handlersFound)
        {
            // try to find
            handlersFound = _applicationHandlers.Find(toSearchFor, _handlerSearch);
            return handlersFound != null && handlersFound.Any();
        }

        private IList<HandlerInfo> Order(IList<HandlerInfo> handlersList)
        {
            if (handlersList.Count == 1)
                return handlersList;

            return _handlerSort.Sort(handlersList);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="toConstructFrom"></param>
        /// <param name="handlersList"></param>
        /// <exception cref="HandlerCannotBeConstructedException"></exception>
        /// <returns></returns>
        private IEnumerable<Type> ConstructTypes(HandlerInfo toConstructFrom, IList<HandlerInfo> handlersList)
        {
            IEnumerable<Type> constructedTypes = null;

            PerformCheck.IsTrue(() => !TryConstructTypes(toConstructFrom, handlersList, out constructedTypes))
                .Throw<HandlerCannotBeConstructedException>(() =>
                    new HandlerCannotBeConstructedException("The handler of type " + toConstructFrom.Type.FullName + " cannot be constructed")
                    );
            PerformCheck.IsTrue(() => !constructedTypes.Any())
                .Throw<HandlerCannotBeConstructedException>(() =>
                    new HandlerCannotBeConstructedException("The handler of type " + toConstructFrom.Type.FullName + " cannot be constructed")
                    );

            return constructedTypes;
        }

        private bool TryConstructTypes(HandlerInfo constructFrom, IList<HandlerInfo> handlers, out IEnumerable<Type> constructedTypes)
        {
            constructedTypes = _typeConstructor.Create(constructFrom, handlers);
            return constructedTypes != null;
        }

        private void RegisterTypesAsHandler<THandler>(IEnumerable<Type> constructedTypes)
        {
            foreach (Type type in constructedTypes)
            {
                // Need to use a named regisration for Unity implementations of IIocContainer
                _iocContainer.Register(typeof(THandler), type, BuildUniqueRegistrationId());
            }
        }

        private static string BuildUniqueRegistrationId()
        {
            return Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Locks performed are handler type specific
        /// </summary>
        /// <typeparam name="T"></typeparam>
        internal static class Lock<T>
        {
            public static readonly ReaderWriterLockSlim _readerWriterLockSlim;

            public static ReaderWriterLockSlim ReaderWriterLockSlim { get { return _readerWriterLockSlim; } }

            static Lock()
            {
                _readerWriterLockSlim = new ReaderWriterLockSlim();
            }
        }

        #endregion Methods
    }
}