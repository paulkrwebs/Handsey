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
        public void ThreadingTest(int iterations, bool clearRegistraionsCache)
        {
            _stopwatch.Restart();

            // parallel loop
            Parallel.For(0, iterations, (i) =>
            {
                // create a new domain object
                TriggerChangeOnADeveloper();

                TriggerChangeOnAnEmployee();

                MapDeveloperViewModelToDevelopViewModel();

                MapTechnicalArchitectToTechnicalArchitectViewModel();

                // optionally clear the cache
                if (clearRegistraionsCache)
                    _integrationContainer.ClearThreadRegistrations();
            });

            _stopwatch.Stop();
            Console.WriteLine("InvokeHandlersForIterations iterations {0}, clearRegistraionsCache {1}, took {2} milliseconds / {3} ticks", iterations, clearRegistraionsCache, _stopwatch.ElapsedMilliseconds, _stopwatch.ElapsedTicks);
        }

        [Test]
        public void TriggerChangeOnADeveloper()
        {
            Developer developer = new Developer("paul", "kiernan", new string[] { "C#" });

            // change it
            developer.Change(developer.FirstName, developer.LastName, new string[] { "C#", "JS" });

            // this won't work in a parralle FIX
            IHandler[] handlerLog = developer.HandlerLog();
            Assert.That(handlerLog.Count(), Is.EqualTo(3));
            Assert.That(handlerLog.Count(h => h.GetType() == typeof(AlertStateChangeHandler<Developer>)), Is.EqualTo(1));
            Assert.That(handlerLog.Count(h => h.GetType() == typeof(SaveStateChangeHandler<Developer>)), Is.EqualTo(1));
            Assert.That(handlerLog.Count(h => h.GetType() == typeof(ProgrammingLanguagesChangedHandler<Developer>)), Is.EqualTo(1));
        }

        [Test]
        public void TriggerChangeOnAnEmployee()
        {
            // create a new domain object
            Employee employee = new Employee("paul", "kiernan");

            // change it
            employee.Change(employee.FirstName, "Mc Kiernan");

            //// this won't work in a parralle FIX
            IHandler[] handlerLog = employee.HandlerLog();
            Assert.That(handlerLog.Count(), Is.EqualTo(2));
            Assert.That(handlerLog.Count(h => h.GetType() == typeof(AlertStateChangeHandler<Employee>)), Is.EqualTo(1));
            Assert.That(handlerLog.Count(h => h.GetType() == typeof(SaveStateChangeHandler<Employee>)), Is.EqualTo(1));
        }

        [Test]
        public void MapDeveloperViewModelToDevelopViewModel()
        {
            // map to a view model
            DeveloperViewModel developerFrom = new DeveloperViewModel();
            DeveloperViewModel developerTo = new DeveloperViewModel();

            InvokeUpdateRequestMappingHandler<DeveloperViewModel, DeveloperViewModel>(developerFrom, developerTo);

            IHandler[] fromHandlerLog = developerFrom.HandlerLog();
            IHandler[] toHandlerLog = developerTo.HandlerLog();

            Assert.That(fromHandlerLog.Count(), Is.EqualTo(1));
            Assert.That(fromHandlerLog.Count(h => h.GetType() == typeof(UpdateRequestHandler<DeveloperViewModel, DeveloperViewModel>)), Is.EqualTo(1));

            Assert.That(toHandlerLog.Count(), Is.EqualTo(1));
            Assert.That(toHandlerLog.Count(h => h.GetType() == typeof(UpdateRequestHandler<DeveloperViewModel, DeveloperViewModel>)), Is.EqualTo(1));
        }

        [Test]
        public void MapTechnicalArchitectToTechnicalArchitectViewModel()
        {
            // map to a view model
            TechnicalArchitect from = new TechnicalArchitect("Len", "Boyde", new[] { "JS", "JAVA", "C#" });
            TechnicalArchitectViewModel to = new TechnicalArchitectViewModel();

            InvokeMappingHandler<TechnicalArchitect, TechnicalArchitectViewModel>(from, to);

            IHandler[] fromHandlerLog = from.HandlerLog();
            IHandler[] toHandlerLog = to.HandlerLog();

            Assert.That(fromHandlerLog.Count(), Is.EqualTo(1));
            Assert.That(fromHandlerLog.Count(h => h.GetType() == typeof(ModelMapperHandler<TechnicalArchitect, TechnicalArchitectViewModel>)), Is.EqualTo(1));

            Assert.That(toHandlerLog.Count(), Is.EqualTo(1));
            Assert.That(toHandlerLog.Count(h => h.GetType() == typeof(ModelMapperHandler<TechnicalArchitect, TechnicalArchitectViewModel>)), Is.EqualTo(1));
        }

        public void InvokeMappingHandler<TFrom, TTo>(TFrom from, TTo to)
        {
            // invoke
            ApplicationLocator
                .Instance
                .Invoke<IOneToOneHandler<TFrom, TTo>>(h => h.Handle(from, to));
        }

        public void InvokeUpdateRequestMappingHandler<TFrom, TTo>(TFrom from, TTo to)
            where TFrom : EmployeeViewModel, new()
            where TTo : EmployeeViewModel, new()
        {
            // invoke
            ApplicationLocator
                .Instance
                .Invoke<IOneToOneHandler<UpdateEmployeeRequest<TFrom>, UpdateEmployeeResponse<TTo>>>(
                h => h.Handle(new UpdateEmployeeRequest<TFrom>() { Employee = from }
                    , new UpdateEmployeeResponse<TTo>() { Employee = to }
                    ));
        }
    }
}