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
    public class MainTest
    {
        private IntegrationContainer _integrationContainer;
        private Stopwatch _stopwatch;

        [SetUp]
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
        public void InvokeHandlersForIterations(int iterations, bool clearRegistraionsCache)
        {
            _stopwatch.Restart();

            // parallel loop
            Parallel.For(0, iterations, (i) =>
            {
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

                InvokeMappingHandler<DeveloperViewModel, DeveloperViewModel>();

                // optionally clear the cache
                if (clearRegistraionsCache)
                    _integrationContainer.ClearThreadRegistrations();
            });

            _stopwatch.Stop();
            Console.WriteLine("InvokeHandlersForIterations iterations {0}, clearRegistraionsCache {1}, took {2} milliseconds / {3} ticks", iterations, clearRegistraionsCache, _stopwatch.ElapsedMilliseconds, _stopwatch.ElapsedTicks);
        }

        public void InvokeMappingHandler<TFrom, TTo>()
            where TFrom : EmployeeViewModel, new()
            where TTo : EmployeeViewModel, new()
        {
            // map to a view model
            TFrom developerFrom = new TFrom();
            TTo developerTo = new TTo();

            // invoke
            ApplicationLocator
                .Instance
                .Invoke<IOneToOneHandler<UpdateEmployeeRequest<TFrom>, UpdateEmployeeResponse<TTo>>>(
                h => h.Handle(new UpdateEmployeeRequest<TFrom>() { Employee = developerFrom }
                    , new UpdateEmployeeResponse<TTo>() { Employee = developerTo }
                    ));

            IHandler[] developerFromHandlerLog = developerFrom.HandlerLog();
            IHandler[] developerToHandlerLog = developerTo.HandlerLog();

            Assert.That(developerFromHandlerLog.Count(), Is.EqualTo(1));
            Assert.That(developerFromHandlerLog.Count(h => h.GetType() == typeof(UpdateRequestHandler<TFrom, TTo>)), Is.EqualTo(1));

            Assert.That(developerToHandlerLog.Count(), Is.EqualTo(1));
            Assert.That(developerToHandlerLog.Count(h => h.GetType() == typeof(UpdateRequestHandler<TFrom, TTo>)), Is.EqualTo(1));
        }
    }
}