using Handsey.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey
{
    public class ApplicationHandlersFactory : IApplicationHandlersFactory
    {
        private readonly IAssemblyWalker _assemblyWalker;
        private readonly IHandlerFactory _handlerFactory;

        public ApplicationHandlersFactory(IAssemblyWalker assemblyWalker, IHandlerFactory handlerFactory)
        {
            _assemblyWalker = assemblyWalker;
            _handlerFactory = handlerFactory;
        }

        public IApplicationHandlers Create(IApplicationConfiguration applicationConfiguration)
        {
            PerformCheck.IsNull(applicationConfiguration).Throw<ArgumentNullException>(() => new ArgumentNullException("Application Configuration cannot be null"));
            PerformCheck.IsTrue(() => CheckFilterConfiguration(applicationConfiguration)).Throw<ArgumentException>(() => new ArgumentException("Please set a handler base type to filter by and a list of assembly name prefixes to filter by."));

            Type[] types = _assemblyWalker.ListAllTypes(applicationConfiguration.BaseType, applicationConfiguration.AssemblyNamePrefixes);

            PerformCheck.IsNull(types).Throw<HandlerNotFoundException>(() => new HandlerNotFoundException("No types that implement the base handler in the application configuration were found"));
            PerformCheck.IsTrue(() => !types.Any()).Throw<HandlerNotFoundException>(() => new HandlerNotFoundException("No types that implement the base handler in the application configuration were found"));

            IList<HandlerInfo> handlers = _handlerFactory.Create(applicationConfiguration.BaseType, types);

            PerformCheck.IsNull(handlers).Throw<HandlerNotFoundException>(() => new HandlerNotFoundException("No handlers were found matching the application configuration"));
            PerformCheck.IsTrue(() => !handlers.Any()).Throw<HandlerNotFoundException>(() => new HandlerNotFoundException("No handlers were found matching the application configuration"));

            return new ApplicationHandlers(handlers);
        }

        private bool CheckFilterConfiguration(IApplicationConfiguration applicationConfiguration)
        {
            return applicationConfiguration.AssemblyNamePrefixes == null
                || applicationConfiguration.AssemblyNamePrefixes.Count() == 0
                || applicationConfiguration.BaseType == null;
        }
    }
}