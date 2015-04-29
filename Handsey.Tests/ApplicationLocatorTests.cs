using Handsey.Handlers;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey.Tests
{
    [TestFixture]
    public class ApplicationLocatorTests
    {
        [Test]
        public void Instance_NoParams_ConfigurationNotSetSoApplicationConfigurationNotSetExceptionThrown()
        {
            Assert.That(() => ApplicationLocator.Instance, Throws.Exception.TypeOf<ApplicationConfigurationNotSetException>());
        }

        [Test]
        public void Instance_NoParams_InstanceBuiltCorrectly()
        {
            ApplicationLocator.Configure(new ApplicationConfiguration(typeof(IHandler), new string[1] { "Handsey.Tests" })
                , new Mock<IIocContainer>().Object);

            Assert.That(ApplicationLocator.Instance, Is.Not.Null);
            Assert.That(ApplicationLocator.Instance, Is.SameAs(ApplicationLocator.Instance));
        }
    }
}