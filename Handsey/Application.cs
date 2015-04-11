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

        public ApplicationHandlers ApplicationHandlers { get; private set; }

        #endregion // Fields

        #region // Constructors

        public Application()
            : this(new AssemblyWalker()
            , new HandlerFactory()
            , new HandlerSearch()
            , new HandlersSort()
            , new TypeConstructor()
            , new ApplicationHandlersFactory())
        { }

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

            // check if tried to find before

            // try to find

            // order

            // construct

            // lock and register
            throw new NotImplementedException("TODO!");
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

        #endregion Methods
    }
}