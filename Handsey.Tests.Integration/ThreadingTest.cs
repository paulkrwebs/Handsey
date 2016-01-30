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
            _stopwatch = new Stopwatch();
            _integrationContainer = new IntegrationContainer();

            _stopwatch.Start();

            ApplicationLocator.Configure(
                    new ApplicationConfiguration(typeof(IHandler)
                    , new string[] { "Handsey.Tests.Integration" }
                    , true)
                    , _integrationContainer);

            _stopwatch.Stop();
            Console.WriteLine("Configuration took {0} milliseconds / {1} ticks", _stopwatch.ElapsedMilliseconds, _stopwatch.ElapsedTicks);
        }

        /// <summary>
        /// This test checks that there are no threading issues with invoking handers. It makes sure the read / write locks are working correcly for reads specifically
        /// There is not expected to be any contention issues running this test
        /// </summary>
        /// <param name="iterations"></param>
        [TestCase(10000)]
        [TestCase(1000000)]
        public void TriggerHandlersIterationsTest(int iterations)
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
            });

            _stopwatch.Stop();
            Console.WriteLine("TriggerHandlersIterationsTest iterations {0}, took {1} milliseconds / {2} ticks", iterations, _stopwatch.ElapsedMilliseconds, _stopwatch.ElapsedTicks);
        }
    }
}