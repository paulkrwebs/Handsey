using Handsey.Handlers;
using Handsey.Tests.Integration.Handlers;
using Handsey.Tests.Integration.IocContainers;
using Handsey.Tests.Integration.Models;
using Handsey.Tests.Integration.ViewModels;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey.Tests.Integration
{
    [TestFixture]
    public class MainTest
    {
        private IHandlerCallLog _handlerCallLog;
        private IntegrationContainer _integrationContainer;

        [SetUp]
        public void Setup()
        {
            _handlerCallLog = new HandlerCallLog();
            _integrationContainer = new IntegrationContainer(_handlerCallLog);

            ApplicationLocator.Configure(
                    new ApplicationConfiguration(typeof(IHandler)
                    , new string[] { "Handsey.Tests.Integration" })
                    , _integrationContainer);
        }

        [TestCase(10)]
        public void InvokeHandlersForIterations(int iterations)
        {
            // parallel loop
            Parallel.For(0, iterations, (i) =>
            {
                int iteration = i + 1;
                _handlerCallLog.Log = new List<Type>();

                // create a new domain object
                Developer developer = new Developer("paul", "kiernan", new string[] { "C#" });

                // change it
                developer.Change(developer.FirstName, developer.LastName, new string[] { "C#", "JS" });

                // this won't work in a parralle FIX

                Assert.That(_handlerCallLog.Log.Count, Is.EqualTo(3));
                Assert.That(_handlerCallLog.Log.Contains(typeof(AlertStateChangeHandler<Developer>)), Is.True);
                Assert.That(_handlerCallLog.Log.Contains(typeof(SaveStateChangeHandler<Developer>)), Is.True);
                Assert.That(_handlerCallLog.Log.Contains(typeof(ProgrammingLanguagesChangedHandler<Developer>)), Is.True);

                // create a new domain object
                Employee employee = new Employee("paul", "kiernan");

                // change it
                employee.Change(employee.FirstName, "Mc Kiernan");

                // this won't work in a parralle FIX
                Assert.That(_handlerCallLog.Log.Count, Is.EqualTo(5));
                Assert.That(_handlerCallLog.Log.Contains(typeof(AlertStateChangeHandler<Employee>)), Is.True);
                Assert.That(_handlerCallLog.Log.Contains(typeof(SaveStateChangeHandler<Employee>)), Is.True);

                // need to stop the IntegrationContainer registering the same handlers twice
                // OR we should use the specific type
                _integrationContainer.ClearRegistrations();
                // map to a view model
            });
        }
    }
}