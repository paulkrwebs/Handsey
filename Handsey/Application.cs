using Handsey.Handlers;
using Handsey.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey
{
    public class Application : IApplicaton
    {
        #region // Fields

        private readonly IAssemblyWalker _assemblyWalker;
        private readonly IHandlerFactory _handlerFactory;
        private readonly IHandlerSearch _handlerSearch;
        private readonly IHandlersSort _handlerSort;
        private readonly ITypeConstructor _typeConstructor;
        private readonly IApplicationHandlersFactory _applicationHandlersFactory;
        private bool _isInitialised;

        public IApplicationConfiguration ApplicationConfiguration { get; set; }

        public IApplicationHandlers ApplicationHandlers { get; private set; }

        public static IApplicaton Instance { get { throw new NotImplementedException("Need to implement a singleton"); } }

        #endregion // Fields

        #region // Constructors

        public Application(IAssemblyWalker assemblyWalker
            , IHandlerFactory handlerFactory
            , IHandlerSearch handlerSearch
            , IHandlersSort handlerSort
            , ITypeConstructor typeConstructor,
             IApplicationHandlersFactory applicationHandlersFactory)
        {
            _assemblyWalker = assemblyWalker;
            _handlerFactory = handlerFactory;
            _handlerSearch = handlerSearch;
            _handlerSort = handlerSort;
            _typeConstructor = typeConstructor;
            _applicationHandlersFactory = applicationHandlersFactory;
        }

        #endregion // Constructors

        #region Methods

        public void Initialise()
        {
            PerformCheck.IsNull(ApplicationConfiguration).Throw<ArgumentNullException>(() => new ArgumentNullException("Please set an application configuration before trying to initialise"));
            PerformCheck.IsNull(ApplicationConfiguration.IocConatainer).Throw<ArgumentNullException>(() => new ArgumentNullException("Please set an ioc container in the application configuration before trying to initialise"));
            PerformCheck.IsTrue(() => CheckFilterConfiguration()).Throw<ArgumentException>(() => new ArgumentException("Please set a handler base type to filter by and a list of assembly name prefixes to filter by."));

            Type[] types = _assemblyWalker.ListAllTypes(ApplicationConfiguration.BaseType, ApplicationConfiguration.AssemblyNamePrefixes);
            IList<HandlerInfo> handlers = _handlerFactory.Create(ApplicationConfiguration.BaseType, types);

            ApplicationHandlers = _applicationHandlersFactory.Create(handlers);
            _isInitialised = true;
        }

        private bool CheckFilterConfiguration()
        {
            return ApplicationConfiguration.AssemblyNamePrefixes == null
                || ApplicationConfiguration.AssemblyNamePrefixes.Count() == 0
                || ApplicationConfiguration.BaseType == null;
        }

        public void Invoke<THandler>(Action<THandler> trigger)
        {
            // make sure the application is initialised
            PerformCheck.IsTrue(() => !_isInitialised).Throw<InvalidOperationException>(() => new InvalidOperationException("Application must be initialised before calling Invoke"));

            // try to resolve
            if (TryInvoke(ApplicationConfiguration.IocConatainer.ResolveAll<THandler>(), trigger))
                return;

            // create handler
            HandlerInfo toSearchFor = CreateHandler<THandler>();

            // Find
            IList<HandlerInfo> handlersList = FindHandlers<THandler>(toSearchFor);

            // order
            handlersList = Order(handlersList);

            // construct
            IEnumerable<Type> constructedTypes = ConstructTypes(toSearchFor, handlersList);

            // register types
            RegisterTypes<THandler>(constructedTypes, trigger);
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

            PerformCheck.IsTrue(() => !TryCreateHandler<THandler>(ApplicationConfiguration.BaseType, out toSearchFor)).Throw<RequestedHandlerNotValidException>(() => new RequestedHandlerNotValidException("Requested handler " + typeof(THandler).FullName + " cannot be converted to a handler."));

            return toSearchFor;
        }

        private bool TryCreateHandler<THandler>(Type baseType, out HandlerInfo handlerInfo)
        {
            handlerInfo = _handlerFactory.Create(ApplicationConfiguration.BaseType, typeof(THandler));
            return handlerInfo != null;
        }

        /// <summary>
        /// Finds handlers
        /// </summary>
        /// <typeparam name="THandler"></typeparam>
        /// <param name="handlerTypeName"></param>
        /// <param name="toSearchFor"></param>
        /// <exception cref="HandlerNotFoundException"></exception>
        /// <returns></returns>
        private IList<HandlerInfo> FindHandlers<THandler>(HandlerInfo toSearchFor)
        {
            IEnumerable<HandlerInfo> handlersFound = null;
            PerformCheck.IsTrue(() => !TryFindHandlers<THandler>(toSearchFor, out handlersFound)).Throw<HandlerNotFoundException>(() => new HandlerNotFoundException("The handler of type " + typeof(THandler).FullName + " could not be found"));

            // check some are returned
            IList<HandlerInfo> handlersList = handlersFound.ToList();
            PerformCheck.IsTrue(() => !handlersList.Any()).Throw<HandlerNotFoundException>(() => new HandlerNotFoundException("The handler of type " + typeof(THandler).FullName + " could not be found"));

            return handlersList;
        }

        /// <summary>
        /// Finds handlers in the ApplicationHandlers object
        /// </summary>
        /// <param name="toSearchFor"></param>
        /// <returns></returns>
        private bool TryFindHandlers<THandler>(HandlerInfo toSearchFor, out IEnumerable<HandlerInfo> handlersFound)
        {
            // try to find
            handlersFound = ApplicationHandlers.Find(toSearchFor, _handlerSearch);
            return handlersFound != null;
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

            PerformCheck.IsTrue(() => !TryConstructTypes(toConstructFrom, handlersList, out constructedTypes)).Throw<HandlerCannotBeConstructedException>(() => new HandlerCannotBeConstructedException("The handler of type " + toConstructFrom.Type.FullName + " cannot be constructed"));
            PerformCheck.IsTrue(() => !constructedTypes.Any()).Throw<HandlerCannotBeConstructedException>(() => new HandlerCannotBeConstructedException("The handler of type " + toConstructFrom.Type.FullName + " cannot be constructed"));

            return constructedTypes;
        }

        private bool TryConstructTypes(HandlerInfo constructFrom, IList<HandlerInfo> handlers, out IEnumerable<Type> constructedTypes)
        {
            constructedTypes = _typeConstructor.Create(constructFrom, handlers);
            return constructedTypes != null;
        }

        private void RegisterTypes<THandler>(IEnumerable<Type> constructedTypes, Action<THandler> trigger)
        {
            // TODO NEED To TEST THE FIRST TRY INVOKE!

            lock (Lock<THandler>.Semaphore)
            {
                // Make sure the handlers haven't been registered in another thread.
                // If it has been registered then invoke
                if (TryInvoke(ApplicationConfiguration.IocConatainer.ResolveAll<THandler>(), trigger))
                    return;

                // If it hasn't then register
                RegisterTypes<THandler>(constructedTypes);
            }

            // Construct from the IocContainer to inject dependencies.
            TryInvoke(ApplicationConfiguration.IocConatainer.ResolveAll<THandler>(), trigger);
        }

        private void RegisterTypes<THandler>(IEnumerable<Type> constructedTypes)
        {
            foreach (Type type in constructedTypes)
            {
                ApplicationConfiguration.IocConatainer.Register(typeof(THandler), type);
            }
        }

        private static class Lock<T>
        {
            public static readonly object Semaphore = new object();
        }

        #endregion Methods
    }
}