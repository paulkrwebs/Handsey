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
    public class OneToOneHandlerTests
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
        public static void ModelMapperHandler_MapTechnicalArchitectToTechnicalArchitectViewModel()
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

        [Test]
        public static void ModelMapperHandler_MapTechnicalArchitectToDeveloperViewModel()
        {
            // map to a view model
            TechnicalArchitect from = new TechnicalArchitect("Len", "Boyde", new[] { "JS", "JAVA", "C#" });
            DeveloperViewModel to = new DeveloperViewModel();

            InvokeMappingHandler<TechnicalArchitect, DeveloperViewModel>(from, to);

            IHandler[] fromHandlerLog = from.HandlerLog();
            IHandler[] toHandlerLog = to.HandlerLog();

            Assert.That(fromHandlerLog.Count(), Is.EqualTo(1));
            Assert.That(fromHandlerLog.Count(h => h.GetType() == typeof(ModelMapperHandler<TechnicalArchitect, DeveloperViewModel>)), Is.EqualTo(1));

            Assert.That(toHandlerLog.Count(), Is.EqualTo(1));
            Assert.That(toHandlerLog.Count(h => h.GetType() == typeof(ModelMapperHandler<TechnicalArchitect, DeveloperViewModel>)), Is.EqualTo(1));
        }

        [Test]
        public static void UpdateRequestHandler_MapDeveloperViewModelToDevelopViewModel()
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

        public static void InvokeMappingHandler<TFrom, TTo>(TFrom from, TTo to)
        {
            // invoke
            ApplicationLocator
                .Instance
                .Invoke<IOneToOneHandler<TFrom, TTo>>(h => h.Handle(from, to));
        }

        public static void InvokeUpdateRequestMappingHandler<TFrom, TTo>(TFrom from, TTo to)
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