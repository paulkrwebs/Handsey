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
        private IntegrationContainer _integrationContainer;

        [SetUp]
        public void Setup()
        {
            _integrationContainer = new IntegrationContainer();

            ApplicationLocator.Configure(
                    new ApplicationConfiguration(typeof(IHandler)
                    , new string[] { "Handsey.Tests.Integration" })
                    , _integrationContainer);
        }

        [TestCase(10000)]
        public void InvokeHandlersForIterations(int iterations)
        {
            // parallel loop
            Parallel.For(0, iterations, (i) =>
            {
                int iteration = i + 1;

                // create a new domain object
                Developer developer = new Developer("paul", "kiernan", new string[] { "C#" });

                // change it
                developer.Change(developer.FirstName, developer.LastName, new string[] { "C#", "JS" });

                // this won't work in a parralle FIX
                IHandler[] developerHandlerLog = developer.HandlerLog();
                Assert.That(developerHandlerLog.Count(), Is.EqualTo(3));
                Assert.That(developerHandlerLog.Count(h => h.GetType() == typeof(AlertStateChangeHandler<Developer>)), Is.EqualTo(1));
                Assert.That(developerHandlerLog.Count(h => h.GetType() == typeof(SaveStateChangeHandler<Developer>)), Is.EqualTo(1));
                Assert.That(developerHandlerLog.Count(h => h.GetType() == typeof(ProgrammingLanguagesChangedHandler<Developer>)), Is.EqualTo(1));

                // create a new domain object
                Employee employee = new Employee("paul", "kiernan");

                // change it
                employee.Change(employee.FirstName, "Mc Kiernan");

                //// this won't work in a parralle FIX
                IHandler[] employeeHandlerLog = employee.HandlerLog();
                Assert.That(employeeHandlerLog.Count(), Is.EqualTo(2));
                Assert.That(employeeHandlerLog.Count(h => h.GetType() == typeof(AlertStateChangeHandler<Employee>)), Is.EqualTo(1));
                Assert.That(employeeHandlerLog.Count(h => h.GetType() == typeof(SaveStateChangeHandler<Employee>)), Is.EqualTo(1));

                //
                _integrationContainer.ClearThreadRegistrations();
                // map to a view model
            });
        }
    }
}