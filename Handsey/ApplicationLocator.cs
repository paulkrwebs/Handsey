using Handsey.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey
{
    public static class ApplicationLocator
    {
        private static volatile IApplicaton _instance;
        private static volatile IApplicationConfiguration _applicationConfiguration;
        private static volatile IIocContainer _iocContainer;
        private static object _instanceSyncLock = new Object();

        public static void Configure(IApplicationConfiguration applicationConfiguration, IIocContainer iocContainer)
        {
            lock (_instanceSyncLock)
            {
                _applicationConfiguration = applicationConfiguration;
                _iocContainer = iocContainer;
            }
        }

        public static IApplicaton Instance
        {
            get
            {
                // taken from https://msdn.microsoft.com/en-gb/library/ff650316.aspx
                if (_instance == null)
                {
                    lock (_instanceSyncLock)
                    {
                        PerformCheck.IsNull(_applicationConfiguration).Throw<ApplicationConfigurationNotSetException>(() => new ApplicationConfigurationNotSetException("Please call the Configure methods before trying to resolve an instance"));

                        if (_instance == null)
                            _instance = BuildInstance();
                    }
                }
                return _instance;
            }
        }

        private static IApplicaton BuildInstance()
        {
            IAssemblyWalker assemblyWalker = new AssemblyWalker();
            IHandlerFactory handlerFactory = new HandlerFactory(_applicationConfiguration.BaseType);
            IApplicationHandlersFactory applicationHandlersFactory = new ApplicationHandlersFactory(assemblyWalker, handlerFactory);
            IApplicationHandlers applicationHandlers = applicationHandlersFactory.Create(_applicationConfiguration);
            IHandlerSearch handlerSearch = new HandlerSearch();
            ITypeConstructor typeConstructor = new TypeConstructor(handlerSearch);

            return new Application(handlerFactory
                    , handlerSearch
                    , new HandlersSort()
                    , typeConstructor
                    , _iocContainer
                    , applicationHandlers
                    , _applicationConfiguration);
        }
    }
}