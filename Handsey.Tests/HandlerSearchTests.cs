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
    public class HandlerSearchTests
    {
        [Test]
        public void Compare_TypeInfoAndTypeInfo_ExactTypeMatch()
        {
            TypeInfo a = new TypeInfo()
            {
                Type = typeof(TechnicalArchitectMappingHandler)
            };

            TypeInfo b = new TypeInfo()
            {
                Type = typeof(TechnicalArchitectMappingHandler)
            };

            IHandlerSearch search = new HandlerSearch();

            Assert.That(search.Compare(a, b), Is.True);
        }

        [Test]
        public void Compare_TypeInfoAndTypeInfo_ExactTypeNoMatch()
        {
            TypeInfo a = new TypeInfo()
            {
                Type = typeof(TechnicalArchitectMappingHandler)
            };

            TypeInfo b = new TypeInfo()
            {
                Type = typeof(EmployeeHandler)
            };

            IHandlerSearch search = new HandlerSearch();

            Assert.That(search.Compare(a, b), Is.False);
        }

        [Test]
        [TestCase(typeof(IOneToOneDataPopulation<TechnicalArchitect, TechnicalArchitectViewModel>), typeof(TechnicalArchitectMappingHandler))]
        [TestCase(typeof(IOneToOneDataPopulation<TechnicalArchitect, TechnicalArchitectViewModel>), typeof(EmployeeMappingHandler<TechnicalArchitect, TechnicalArchitectViewModel>))]
        [TestCase(typeof(EmployeeMappingHandler<TechnicalArchitect, TechnicalArchitectViewModel>), typeof(EmployeeMappingHandler<TechnicalArchitect, TechnicalArchitectViewModel>))]
        public void Compare_TypeInfoAndTypeInfo_IsAssignableMatch(Type typeA, Type typeB)
        {
            TypeInfo a = new TypeInfo()
            {
                Type = typeA
            };

            TypeInfo b = new TypeInfo()
            {
                Type = typeB
            };

            IHandlerSearch search = new HandlerSearch();

            Assert.That(search.Compare(a, b), Is.True);
        }

        [Test]
        public void Compare_TypeInfoAndTypeInfo_IsAssignableNoMatch()
        {
            TypeInfo a = new TypeInfo()
            {
                Type = typeof(IOneToOneDataPopulation<TechnicalArchitect, TechnicalArchitectViewModel>)
            };

            TypeInfo b = new TypeInfo()
            {
                Type = typeof(EmployeeHandler)
            };

            IHandlerSearch search = new HandlerSearch();

            Assert.That(search.Compare(a, b), Is.False);
        }

        [Test]
        public void Compare_TypeInfoAndTypeInfo_GenericParameterAssingableMatch()
        {
            TypeInfo a = new TypeInfo()
            {
                GenericParametersInfo = CreateGenericParametersWithNoConstraints<Developer, DeveloperViewModel>(),
                Type = typeof(EmployeeMappingHandler<Developer, DeveloperViewModel>)
            };

            TypeInfo b = new TypeInfo()
            {
                GenericParametersInfo = CreateGenericParametersWithNoConstraints<Developer, DeveloperViewModel>(),
                Type = typeof(EmployeeMappingHandler<Employee, EmployeeViewModel>)
            };

            IHandlerSearch search = new HandlerSearch();

            Assert.That(search.Compare(a, b), Is.True);
        }

        [Test]
        public void Compare_TypeInfoAndTypeInfo_GenericParameterAssingableNoMatch()
        {
        }

        [Test]
        public void Compare_TypeInfoAndTypeInfo_GenericParameterConstraintAssingableMatch()
        {
            TypeInfo a = new TypeInfo()
            {
                GenericParametersInfo = CreateGenericParametersWithConstraints<Developer, DeveloperViewModel>(),
                Type = typeof(EmployeeMappingHandler<Developer, DeveloperViewModel>)
            };

            TypeInfo b = new TypeInfo()
            {
                GenericParametersInfo = CreateGenericParametersWithConstraints<Employee, EmployeeViewModel>(),
                Type = typeof(EmployeeMappingHandler<Employee, EmployeeViewModel>)
            };

            IHandlerSearch search = new HandlerSearch();

            Assert.That(search.Compare(a, b), Is.True);
        }

        [Test]
        public void Compare_TypeInfoAndTypeInfo_GenericParameterConstraintAssingableNoMatch()
        { }

        [Test]
        public void Compare_TypeInfoAndTypeInfo_GenericParameterConstraintIsGenericTypeWhichIsConstructedMatch()
        { }

        [Test]
        public void Compare_TypeInfoAndTypeInfo_GenericParameterConstraintIsGenericTypeWhichIsConstructedNoMatch()
        { }

        [Test]
        public void Compare_TypeInfoAndTypeInfo_GenericParameterConstraintIsGenericTypeWhichIsNotConstructedMatch()
        { }

        [Test]
        public void Compare_TypeInfoAndTypeInfo_GenericParameterConstraintIsGenericTypeWhichIsNotConstructedNoMatch()
        { }

        #region // Helpers

        private IList<GenericParameterInfo> CreateGenericParametersWithConstraints<TParam1>()
        {
            return new List<GenericParameterInfo>()
            {
                new GenericParameterInfo() { FilteredContraints = CreateTypesInfo<TParam1>() }
            };
        }

        private IList<GenericParameterInfo> CreateGenericParametersWithNoConstraints<TParam1, TParam2>()
        {
            return new List<GenericParameterInfo>()
            {
                new GenericParameterInfo() { Type = typeof(TParam1) },
                new GenericParameterInfo() { Type = typeof(TParam2) }
            };
        }

        private IList<GenericParameterInfo> CreateGenericParametersWithConstraints<TParam1, TParam2>()
        {
            return new List<GenericParameterInfo>()
            {
                new GenericParameterInfo() { FilteredContraints = CreateTypesInfo<TParam1>(), Type = typeof(TParam1) },
                new GenericParameterInfo() { FilteredContraints = CreateTypesInfo<TParam2>(), Type = typeof(TParam2) }
            };
        }

        private IList<TypeInfo> CreateTypesInfo<TParam1>()
        {
            return new List<TypeInfo>() { CreateTypeInfo<TParam1>() };
        }

        private TypeInfo CreateTypeInfo<TParam1>()
        {
            Type type = typeof(TParam1);

            return new TypeInfo()
            {
                Type = type,
                IsConstructed = type.IsConstructedGenericType || (!type.IsGenericParameter),
                IsGenericType = type.IsGenericType
            };
        }

        #endregion // Helpers
    }
}