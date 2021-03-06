﻿using Handsey.Handlers;
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

            TypeInfo classInfo = _handlerFactory.Create(typeof(IHandles), type);

            Assert.That(classInfo, Is.Not.Null);
            Assert.That(classInfo.IsConstructed, Is.True);
            Assert.That(classInfo.IsGenericType, Is.False);
            Assert.That(classInfo.Type.FullName, Is.EqualTo(type.FullName));
            Assert.That(classInfo.GenericParametersInfo, Is.Empty);
            Assert.That(classInfo.FilteredInterfaces.Count(), Is.EqualTo(1));
        }

        [Test]
        public void Create_TypeAndType_ClassInfoForGenericConstructedType()
        {
            Type type = typeof(EmployeeHandler<Employee>);

            TypeInfo classInfo = _handlerFactory.Create(typeof(IHandles), type);

            Assert.That(classInfo, Is.Not.Null);
            Assert.That(classInfo.IsConstructed, Is.True);
            Assert.That(classInfo.IsGenericType, Is.True);
            Assert.That(classInfo.Type.FullName, Is.EqualTo(type.FullName));
            Assert.That(classInfo.GenericTypeDefinition.FullName, Is.EqualTo(type.GetGenericTypeDefinition().FullName));
            Assert.That(classInfo.GenericParametersInfo.Count(), Is.EqualTo(1));
            Assert.That(classInfo.FilteredInterfaces.Count(), Is.EqualTo(1));
        }

        [Test]
        public void Create_TypeAndType_ClassInfoForGenericeNonConstructedType()
        {
            Type type = typeof(EmployeeHandler<>);

            TypeInfo classInfo = _handlerFactory.Create(typeof(IHandles), type);

            Assert.That(classInfo, Is.Not.Null);
            Assert.That(classInfo.IsConstructed, Is.False);
            Assert.That(classInfo.IsGenericType, Is.True);
            Assert.That(classInfo.Type.FullName, Is.EqualTo(type.FullName));
            Assert.That(classInfo.GenericTypeDefinition.FullName, Is.EqualTo(type.GetGenericTypeDefinition().FullName));
            Assert.That(classInfo.GenericParametersInfo.Count(), Is.EqualTo(1));
            Assert.That(classInfo.FilteredInterfaces.Count(), Is.EqualTo(1));
        }

        [Test]
        public void Create_TypeAndType_ClassInfoForGenericTypeWithGenericParametersNoConstrains()
        {
            Type type = typeof(DeveloperMappingHandler<,>);

            TypeInfo classInfo = _handlerFactory.Create(typeof(IHandles), type);

            Assert.That(classInfo, Is.Not.Null);
            Assert.That(classInfo.GenericParametersInfo.Count(), Is.EqualTo(2));
            Assert.That(classInfo.GenericParametersInfo[0].FilteredContraints, Is.Empty);
            Assert.That(classInfo.GenericParametersInfo[1].FilteredContraints, Is.Empty);
        }

        [Test]
        public void Create_TypeAndType_ClassInfoForGenericTypeWithGenericParametersThatHaveConstrains()
        {
            Type type = typeof(TechnicalEmployeeMappingHandler<,>);

            TypeInfo classInfo = _handlerFactory.Create(typeof(IHandles), type);

            Assert.That(classInfo, Is.Not.Null);
            Assert.That(classInfo.GenericParametersInfo.Count(), Is.EqualTo(2));
            Assert.That(classInfo.GenericParametersInfo[0].FilteredContraints.Count(), Is.EqualTo(1));
            Assert.That(classInfo.GenericParametersInfo[1].FilteredContraints.Count(), Is.EqualTo(1));
            Assert.That(classInfo.GenericParametersInfo[1].FilteredContraints[0].IsConstructed, Is.True);
            Assert.That(classInfo.GenericParametersInfo[1].FilteredContraints[0].IsGenericType, Is.False);
        }

        [Test]
        public void Create_TypeAndType_ClassInfoForGenericTypeWithGenericParametersThatHaveConstrainsThatIsGenericConstructedType()
        {
            Type type = typeof(EmployeePayloadMappingHandler<>);

            TypeInfo classInfo = _handlerFactory.Create(typeof(IHandles), type);

            Assert.That(classInfo, Is.Not.Null);
            Assert.That(classInfo.GenericParametersInfo.Count(), Is.EqualTo(1));
            Assert.That(classInfo.GenericParametersInfo[0].FilteredContraints.Count(), Is.EqualTo(1));
            Assert.That(classInfo.GenericParametersInfo[0].FilteredContraints[0].IsConstructed, Is.True);
            Assert.That(classInfo.GenericParametersInfo[0].FilteredContraints[0].IsGenericType, Is.True);
            Assert.That(classInfo.GenericParametersInfo[0].FilteredContraints[0].GenericParametersInfo.Count(), Is.EqualTo(2));
        }

        [Test]
        public void Create_TypeAndType_ClassInfoForGenericTypeWithGenericParametersThatHaveConstrainsThatIsGenericNotConstructedType()
        {
            Type type = typeof(EmployeePayloadMappingHandler<,,>);

            TypeInfo classInfo = _handlerFactory.Create(typeof(IHandles), type);

            Assert.That(classInfo, Is.Not.Null);
            Assert.That(classInfo.GenericParametersInfo.Count(), Is.EqualTo(3));
            Assert.That(classInfo.GenericParametersInfo[0].FilteredContraints.Count(), Is.EqualTo(1));
            Assert.That(classInfo.GenericParametersInfo[0].FilteredContraints[0].GenericParametersInfo.Count(), Is.EqualTo(2));
            Assert.That(classInfo.GenericParametersInfo[0].FilteredContraints[0].GenericParametersInfo[0].FilteredContraints.Count(), Is.EqualTo(1));
            Assert.That(classInfo.GenericParametersInfo[0].FilteredContraints[0].GenericParametersInfo[1].FilteredContraints.Count(), Is.EqualTo(1));
        }

        [Test]
        public void Create_TypeAndType_ClassInfoForConcreteTypeWithGenericConstructedInterface()
        {
            Type type = typeof(TechnicalArchitectMappingHandler);

            TypeInfo classInfo = _handlerFactory.Create(typeof(IHandles), type);

            Assert.That(classInfo, Is.Not.Null);
            Assert.That(classInfo.GenericParametersInfo.Count(), Is.EqualTo(0));
            Assert.That(classInfo.FilteredInterfaces.Count(), Is.EqualTo(2));
            Assert.That(classInfo.FilteredInterfaces[1].IsConstructed, Is.True);
            Assert.That(classInfo.FilteredInterfaces[1].IsGenericType, Is.True);
            Assert.That(classInfo.FilteredInterfaces[1].GenericParametersInfo[0].Name, Is.EqualTo("TechnicalArchitect"));
            Assert.That(classInfo.FilteredInterfaces[1].GenericParametersInfo[1].Name, Is.EqualTo("TechnicalArchitectViewModel"));
            Assert.That(classInfo.FilteredInterfaces[1].FilteredInterfaces.Count(), Is.EqualTo(0), "Interfaces don't have interface, they appear on the class implementing the interface");
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