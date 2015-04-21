using Handsey.Handlers;
using Handsey.Tests.TestObjects.Handlers;
using Handsey.Tests.TestObjects.Models;
using Handsey.Tests.TestObjects.ViewModels;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            Assert.That(_handlerFactory.Create(typeof(IHandler), null as Type), Is.Null);
        }

        [Test]
        [TestCase(new object[] { typeof(EmployeeMappingHandler<,>), ExecutionOrder.First })]
        [TestCase(new object[] { typeof(TechnicalEmployeeMappingHandler<,>), ExecutionOrder.InSequence })]
        [TestCase(new object[] { typeof(TechnicalArchitectMappingHandler), ExecutionOrder.Last })]
        [TestCase(new object[] { typeof(DeveloperMappingHandler<,>), ExecutionOrder.NotSet })]
        public void Create_TypeAndType_PopulateExecutionOrder(Type type, ExecutionOrder expectedExecutionOrder)
        {
            HandlerInfo classInfo = _handlerFactory.Create(typeof(IHandler), type);

            Assert.That(classInfo, Is.Not.Null);
            Assert.That(classInfo.ExecutionOrder, Is.EqualTo(expectedExecutionOrder));
        }

        [Test]
        public void Create_TypeAndType_PopulateExecutesAfter()
        {
            HandlerInfo classInfo = _handlerFactory.Create(typeof(IHandler), typeof(TechnicalEmployeeMappingHandler<,>));

            Assert.That(classInfo, Is.Not.Null);
            Assert.That(classInfo.ExecutionOrder, Is.EqualTo(ExecutionOrder.InSequence));
            Assert.That(classInfo.ExecutesAfter, Is.Not.Null);
            Assert.That(classInfo.ExecutesAfter.Count(), Is.EqualTo(1));
            Assert.That(classInfo.ExecutesAfter[0], Is.EqualTo(typeof(DeveloperMappingHandler<,>)));
        }

        [Test]
        public void Create_TypeAndType_ClassInfoForConcreateType()
        {
            Type type = typeof(EmployeeHandler);

            HandlerInfo classInfo = _handlerFactory.Create(typeof(IHandler), type);

            Assert.That(classInfo, Is.Not.Null);
            Assert.That(classInfo.IsConstructed, Is.True);
            Assert.That(classInfo.IsGenericType, Is.False);
            Assert.That(classInfo.Type.FullName, Is.EqualTo(type.FullName));
            Assert.That(classInfo.GenericParametersInfo, Is.Empty);
            Assert.That(classInfo.FilteredInterfaces.Count(), Is.EqualTo(1));
        }

        [Test]
        public void Create_TypeAndType_ClassInfoForGenericConstructedTypeReferenceTypeParameterWithDefaultConstructore()
        {
            Type type = typeof(IHandler<Employee>);

            HandlerInfo classInfo = _handlerFactory.Create(typeof(IHandler), type);

            Assert.That(classInfo, Is.Not.Null);
            Assert.That(classInfo.IsConstructed, Is.True);
            Assert.That(classInfo.IsGenericType, Is.True);
            Assert.That(classInfo.Type.FullName, Is.EqualTo(type.FullName));
            Assert.That(classInfo.GenericTypeDefinition.FullName, Is.EqualTo(type.GetGenericTypeDefinition().FullName));
            Assert.That(classInfo.GenericParametersInfo.Count(), Is.EqualTo(1));
            Assert.That(classInfo.GenericParametersInfo[0].IsValueType, Is.EqualTo(false));
            Assert.That(classInfo.GenericParametersInfo[0].HasDefaultConstuctor, Is.EqualTo(true));
        }

        [Test]
        public void Create_TypeAndType_ClassInfoForGenericConstructedTypeReferenceTypeParameterWithoutDefaultConstructore()
        {
            Type type = typeof(IHandler<NoDefaultConstructor>);

            HandlerInfo classInfo = _handlerFactory.Create(typeof(IHandler), type);

            Assert.That(classInfo, Is.Not.Null);
            Assert.That(classInfo.IsConstructed, Is.True);
            Assert.That(classInfo.IsGenericType, Is.True);
            Assert.That(classInfo.Type.FullName, Is.EqualTo(type.FullName));
            Assert.That(classInfo.GenericTypeDefinition.FullName, Is.EqualTo(type.GetGenericTypeDefinition().FullName));
            Assert.That(classInfo.GenericParametersInfo.Count(), Is.EqualTo(1));
            Assert.That(classInfo.GenericParametersInfo[0].IsValueType, Is.EqualTo(false));
            Assert.That(classInfo.GenericParametersInfo[0].HasDefaultConstuctor, Is.EqualTo(false));
        }

        [Test]
        public void Create_TypeAndType_ClassInfoForGenericConstructedTypeValueTypeParameter()
        {
            Type type = typeof(IHandler<int>);

            HandlerInfo classInfo = _handlerFactory.Create(typeof(IHandler), type);

            Assert.That(classInfo, Is.Not.Null);
            Assert.That(classInfo.IsConstructed, Is.True);
            Assert.That(classInfo.IsGenericType, Is.True);
            Assert.That(classInfo.Type.FullName, Is.EqualTo(type.FullName));
            Assert.That(classInfo.GenericTypeDefinition.FullName, Is.EqualTo(type.GetGenericTypeDefinition().FullName));
            Assert.That(classInfo.GenericParametersInfo.Count(), Is.EqualTo(1));
            Assert.That(classInfo.GenericParametersInfo[0].IsValueType, Is.EqualTo(true));
            Assert.That(classInfo.GenericParametersInfo[0].HasDefaultConstuctor, Is.EqualTo(false));
        }

        [Test]
        public void Create_TypeAndType_ClassInfoForGenericeNonConstructedType()
        {
            Type type = typeof(EmployeeHandler<>);

            HandlerInfo classInfo = _handlerFactory.Create(typeof(IHandler), type);

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

            HandlerInfo classInfo = _handlerFactory.Create(typeof(IHandler), type);

            Assert.That(classInfo, Is.Not.Null);
            Assert.That(classInfo.GenericParametersInfo.Count(), Is.EqualTo(2));
            Assert.That(classInfo.GenericParametersInfo[0].FilteredContraints, Is.Empty);
            Assert.That(classInfo.GenericParametersInfo[1].FilteredContraints, Is.Empty);
        }

        [Test]
        public void Create_TypeAndType_ClassInfoForGenericTypeWithGenericParametersThatHaveConstrains()
        {
            Type type = typeof(TechnicalEmployeeMappingHandler<,>);

            HandlerInfo classInfo = _handlerFactory.Create(typeof(IHandler), type);

            Assert.That(classInfo, Is.Not.Null);
            Assert.That(classInfo.GenericParametersInfo.Count(), Is.EqualTo(2));
            Assert.That(classInfo.GenericParametersInfo[0].FilteredContraints.Count(), Is.EqualTo(1));
            Assert.That(classInfo.GenericParametersInfo[1].FilteredContraints.Count(), Is.EqualTo(1));
            Assert.That(classInfo.GenericParametersInfo[1].FilteredContraints[0].IsConstructed, Is.True);
            Assert.That(classInfo.GenericParametersInfo[1].FilteredContraints[0].IsGenericType, Is.False);
        }

        [Test]
        public void Create_TypeAndType_ClassInfoForGenericTypeWithGenericParametersThatHaveConstrainsAndASpecialConstrains()
        {
            Type type = typeof(VersionableHandler<>);

            HandlerInfo classInfo = _handlerFactory.Create(typeof(IHandler), type);

            Assert.That(classInfo, Is.Not.Null);
            Assert.That(classInfo.GenericParametersInfo.Count(), Is.EqualTo(1));
            Assert.That(classInfo.GenericParametersInfo[0].FilteredContraints.Count(), Is.EqualTo(1));
            Assert.That(classInfo.GenericParametersInfo[0].FilteredContraints[0].IsConstructed, Is.True);
            Assert.That(classInfo.GenericParametersInfo[0].FilteredContraints[0].IsGenericType, Is.False);
            Assert.That(classInfo.GenericParametersInfo[0].FilteredContraints[0].Type, Is.EqualTo(typeof(IVersionable)));
            Assert.That(classInfo.GenericParametersInfo[0].SpecialConstraint, Is.EqualTo(GenericParameterAttributes.DefaultConstructorConstraint | GenericParameterAttributes.ReferenceTypeConstraint));
        }

        [Test]
        public void Create_TypeAndType_ClassInfoForGenericTypeWithGenericParametersThatHaveConstrainsThatIsGenericConstructedType()
        {
            Type type = typeof(EmployeePayloadMappingHandler<>);

            HandlerInfo classInfo = _handlerFactory.Create(typeof(IHandler), type);

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

            HandlerInfo classInfo = _handlerFactory.Create(typeof(IHandler), type);

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

            HandlerInfo classInfo = _handlerFactory.Create(typeof(IHandler), type);

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
            Assert.That(_handlerFactory.Create(typeof(IHandler), null as Type[]), Is.Empty);
        }

        [Test]
        public void Create_TypeAndTypeArray_AllTypesCreated()
        {
            List<Type> types = new List<Type>();

            types.Add(typeof(EmployeeHandler));
            types.Add(typeof(EmployeeHandler<>));

            Assert.That(_handlerFactory.Create(typeof(IHandler), types.ToArray()).Count, Is.EqualTo(2));
        }

        #region // test objects

        public class NoDefaultConstructor
        {
            public NoDefaultConstructor(int param1)
            { }
        }

        #endregion // test objects
    }
}