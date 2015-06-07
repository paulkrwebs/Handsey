using Handsey.Handlers;
using Handsey.Tests.Integration.Handlers;
using Handsey.Tests.Integration.IocContainers;
using Handsey.Tests.Integration.Models;
using Handsey.Tests.Integration.ViewModels;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey.Tests.Integration
{
    [TestFixture]
    public class ThreadingTest
    {
        private IntegrationContainer _integrationContainer;
        private Stopwatch _stopwatch;

        [TestFixtureSetUp]
        public void Setup()
        {
            _integrationContainer = new IntegrationContainer();
            _stopwatch = new Stopwatch();

            _stopwatch.Start();

            ApplicationLocator.Configure(
                    new ApplicationConfiguration(typeof(IHandler)
                    , new string[] { "Handsey.Tests.Integration" })
                    , _integrationContainer);

            _stopwatch.Stop();
            Console.WriteLine("Configuration took {0} milliseconds / {1} ticks", _stopwatch.ElapsedMilliseconds, _stopwatch.ElapsedTicks);
        }

        [TestCase(10000, false)]
        [TestCase(10000, true)]
        [TestCase(1000000, false)]
        [TestCase(1000000, true)]
        public void IterationsTest(int iterations, bool clearRegistraionsCache)
        {
            _stopwatch.Restart();

            // parallel loop
            Parallel.For(0, iterations, (i) =>
            {
                // create a new domain object
                ChangeHandlerTests.TriggerChangeOnADeveloper();

                ChangeHandlerTests.TriggerChangeOnAnEmployee();

                ChangeHandlerTests.TriggerMultipleChangesOnAnSupportTicket();

                OneToOneHandlerTests.UpdateRequestHandler_MapDeveloperViewModelToDevelopViewModel();

                OneToOneHandlerTests.ModelMapperHandler_MapTechnicalArchitectToTechnicalArchitectViewModel();

                // optionally clear the cache
                if (clearRegistraionsCache)
                    _integrationContainer.ClearThreadRegistrations();
            });

            _stopwatch.Stop();
            Console.WriteLine("InvokeHandlersForIterations iterations {0}, clearRegistraionsCache {1}, took {2} milliseconds / {3} ticks", iterations, clearRegistraionsCache, _stopwatch.ElapsedMilliseconds, _stopwatch.ElapsedTicks);
        }
    }
}