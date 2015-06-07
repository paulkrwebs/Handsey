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
    public class ChangeHandlerTests
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

        [Test]
        public static void TriggerChangeOnADeveloper()
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
        public static void TriggerChangeOnAnEmployee()
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
        public static void TriggerMultipleChangesOnAnSupportTicket()
        {
            // create a new domain object
            SupportTicket supportTicket = new SupportTicket(new Project()
                , "Test"
                , SupportTicketPriority.High
                , SupportTicketStatus.Open
                , SupportTicketResolution.NotSet
                , "description"
                , new Employee("Jon", "Doe")
                , new Employee("Jane", "Doe"));

            // change it
            supportTicket.AssignTo(new Employee("paul", "kiernan"));
            supportTicket.Resolve(SupportTicketResolution.Completed);
            supportTicket.ReOpen();
            supportTicket.Resolve(SupportTicketResolution.WontFix);
            supportTicket.ReOpen();

            //// this won't work in a parralle FIX
            IHandler[] handlerLog = supportTicket.HandlerLog();
            Assert.That(handlerLog.Count(), Is.EqualTo(10));
            Assert.That(handlerLog.Count(h => h.GetType() == typeof(AlertStateChangeHandler<SupportTicket>)), Is.EqualTo(5));
            Assert.That(handlerLog.Count(h => h.GetType() == typeof(SaveStateChangeHandler<SupportTicket>)), Is.EqualTo(5));
        }
    }
}