using Handsey.Handlers;
using Handsey.Tests.TestObjects.Handlers;
using Handsey.Tests.TestObjects.Models;
using Handsey.Tests.TestObjects.ViewModels;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey.Tests
{
    [TestFixture]
    public class HandlerFactoryTests
    {
        private IHandlerFactory _handlerFactory;

        [SetUp]
        public void StartUp()
        {
            _handlerFactory = new HandlerFactory();
        }

        [Test]
        public void Create_TypeAndType_TypeIsNullSoNullReturned()
        {
            Assert.That(_handlerFactory.Create(typeof(IHandles), null as Type), Is.Null);
        }

        [Test]
        public void Create_TypeAndType_ClassInfoForConcreateType()
        {
            Type type = typeof(EmployeeHandler);

            ClassInfo classInfo = _handlerFactory.Create(typeof(IHandles), type);

            Assert.That(classInfo, Is.Not.Null);
            Assert.That(classInfo.IsConstructed, Is.True);
            Assert.That(classInfo.IsGenericType, Is.False);
            Assert.That(classInfo.Type.FullName, Is.EqualTo(type.FullName));
            Assert.That(classInfo.GenericParameterInfo, Is.Empty);
            Assert.That(classInfo.FilteredInterfaces.Count(), Is.EqualTo(1));
        }

        [Test]
        public void Create_TypeAndType_ClassInfoForGenericConstructedType()
        {
            Type type = typeof(EmployeeHandler<Employee>);

            ClassInfo classInfo = _handlerFactory.Create(typeof(IHandles), type);

            Assert.That(classInfo, Is.Not.Null);
            Assert.That(classInfo.IsConstructed, Is.True);
            Assert.That(classInfo.IsGenericType, Is.True);
            Assert.That(classInfo.Type.FullName, Is.EqualTo(type.FullName));
            Assert.That(classInfo.GenericParameterInfo.Count(), Is.EqualTo(1));
            Assert.That(classInfo.FilteredInterfaces.Count(), Is.EqualTo(1));
        }

        [Test]
        public void Create_TypeAndType_ClassInfoForGenericeNonConstructedType()
        {
            Type type = typeof(EmployeeHandler<>);

            ClassInfo classInfo = _handlerFactory.Create(typeof(IHandles), type);

            Assert.That(classInfo, Is.Not.Null);
            Assert.That(classInfo.IsConstructed, Is.False);
            Assert.That(classInfo.IsGenericType, Is.True);
            Assert.That(classInfo.Type.FullName, Is.EqualTo(type.FullName));
            Assert.That(classInfo.GenericParameterInfo.Count(), Is.EqualTo(1));
            Assert.That(classInfo.FilteredInterfaces.Count(), Is.EqualTo(1));
        }

        [Test]
        public void Create_TypeAndType_ClassInfoForGenericTypeWithGenericParametersNoConstrains()
        {
            Type type = typeof(DeveloperMappingHandler<,>);

            ClassInfo classInfo = _handlerFactory.Create(typeof(IHandles), type);

            Assert.That(classInfo, Is.Not.Null);
            Assert.That(classInfo.GenericParameterInfo.Count(), Is.EqualTo(2));
            Assert.That(classInfo.GenericParameterInfo[0].FilteredContraints, Is.Empty);
            Assert.That(classInfo.GenericParameterInfo[1].FilteredContraints, Is.Empty);
        }

        [Test]
        public void Create_TypeAndType_ClassInfoForGenericTypeWithGenericParametersThatHaveConstrains()
        {
            Type type = typeof(TechnicalEmployeeMappingHandler<,>);

            ClassInfo classInfo = _handlerFactory.Create(typeof(IHandles), type);

            Assert.That(classInfo, Is.Not.Null);
            Assert.That(classInfo.GenericParameterInfo.Count(), Is.EqualTo(2));
            Assert.That(classInfo.GenericParameterInfo[0].FilteredContraints.Count(), Is.EqualTo(1));
            Assert.That(classInfo.GenericParameterInfo[1].FilteredContraints.Count(), Is.EqualTo(1));
            Assert.That(classInfo.GenericParameterInfo[1].FilteredContraints[0].IsConstructed, Is.True);
            Assert.That(classInfo.GenericParameterInfo[1].FilteredContraints[0].IsGenericType, Is.False);
        }

        [Test]
        public void Create_TypeAndType_ClassInfoForGenericTypeWithGenericParametersThatHaveConstrainsThatIsGenericConstructedType()
        {
            Type type = typeof(EmployeePayloadMappingHandler<>);

            ClassInfo classInfo = _handlerFactory.Create(typeof(IHandles), type);

            Assert.That(classInfo, Is.Not.Null);
            Assert.That(classInfo.GenericParameterInfo.Count(), Is.EqualTo(1));
            Assert.That(classInfo.GenericParameterInfo[0].FilteredContraints.Count(), Is.EqualTo(1));
            Assert.That(classInfo.GenericParameterInfo[0].FilteredContraints[0].IsConstructed, Is.True);
            Assert.That(classInfo.GenericParameterInfo[0].FilteredContraints[0].IsGenericType, Is.True);
            Assert.That(classInfo.GenericParameterInfo[0].FilteredContraints[0].GenericParameterInfo.Count(), Is.EqualTo(2));
        }

        [Test]
        public void Create_TypeAndType_ClassInfoForGenericTypeWithGenericParametersThatHaveConstrainsThatIsGenericNotConstructedType()
        {
            Type type = typeof(EmployeePayloadMappingHandler<,,>);

            ClassInfo classInfo = _handlerFactory.Create(typeof(IHandles), type);

            Assert.That(classInfo, Is.Not.Null);
            Assert.That(classInfo.GenericParameterInfo.Count(), Is.EqualTo(3));
            Assert.That(classInfo.GenericParameterInfo[0].FilteredContraints.Count(), Is.EqualTo(1));
            Assert.That(classInfo.GenericParameterInfo[0].FilteredContraints[0].GenericParameterInfo.Count(), Is.EqualTo(2));
            Assert.That(classInfo.GenericParameterInfo[0].FilteredContraints[0].GenericParameterInfo[0].FilteredContraints.Count(), Is.EqualTo(1));
            Assert.That(classInfo.GenericParameterInfo[0].FilteredContraints[0].GenericParameterInfo[1].FilteredContraints.Count(), Is.EqualTo(1));
        }

        [Test]
        public void Create_TypeAndTypeArray_ypeArrayIsNullSoEmptyReturned()
        {
            Assert.That(_handlerFactory.Create(typeof(IHandles), null as Type[]), Is.Empty);
        }

        [Test]
        public void Create_TypeAndTypeArray_AllTypesCreated()
        {
            List<Type> types = new List<Type>();

            types.Add(typeof(EmployeeHandler));
            types.Add(typeof(EmployeeHandler<>));

            Assert.That(_handlerFactory.Create(typeof(IHandles), types.ToArray()).Count, Is.EqualTo(2));
        }
    }
}