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
        public void Compare_TypeInfoAndTypeInfo_NullCheck()
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

            Assert.That(search.Compare(null, b), Is.False);
            Assert.That(search.Compare(a, null), Is.False);
            Assert.That(search.Compare(new TypeInfo(), b), Is.False);
            Assert.That(search.Compare(a, new TypeInfo()), Is.False);
        }

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
                GenericParametersInfo = CreateGenericParameters<Developer, DeveloperViewModel>(),
                Type = typeof(EmployeeMappingHandler<Developer, DeveloperViewModel>)
            };

            TypeInfo b = new TypeInfo()
            {
                GenericParametersInfo = CreateGenericParameters<Developer, DeveloperViewModel>(),
                Type = typeof(EmployeeMappingHandler<Employee, EmployeeViewModel>)
            };

            IHandlerSearch search = new HandlerSearch();

            Assert.That(search.Compare(a, b), Is.True);
        }

        [Test]
        public void Compare_TypeInfoAndTypeInfo_GenericParameterAssingableNoMatch()
        {
            TypeInfo a = new TypeInfo()
            {
                GenericParametersInfo = CreateGenericParameters<Developer, DeveloperViewModel>(),
                Type = typeof(EmployeeMappingHandler<Developer, DeveloperViewModel>)
            };

            TypeInfo b = new TypeInfo()
            {
                GenericParametersInfo = CreateGenericParameters<TechnicalArchitect, TechnicalArchitectViewModel>(),
                Type = typeof(EmployeeMappingHandler<Employee, EmployeeViewModel>)
            };

            IHandlerSearch search = new HandlerSearch();

            Assert.That(search.Compare(a, b), Is.False);
        }

        [Test]
        public void Compare_TypeInfoAndTypeInfo_GenericParameterConstraintAssingableMatch()
        {
            TypeInfo a = new TypeInfo()
            {
                GenericParametersInfo = CreateGenericParameters<Developer, DeveloperViewModel>(),
                Type = typeof(EmployeeMappingHandler<Developer, DeveloperViewModel>)
            };

            TypeInfo b = new TypeInfo()
            {
                GenericParametersInfo = CreateGenericParametersWithConstraints<Employee, EmployeeViewModel>(),
                Type = typeof(EmployeeMappingHandler<,>)
            };

            IHandlerSearch search = new HandlerSearch();

            Assert.That(search.Compare(a, b), Is.True);
        }

        [Test]
        public void Compare_TypeInfoAndTypeInfo_GenericParameterConstraintAssingableNoMatch()
        {
            TypeInfo a = new TypeInfo()
            {
                GenericParametersInfo = CreateGenericParameters<Employee, EmployeeViewModel>(),
                Type = typeof(EmployeeMappingHandler<Employee, EmployeeViewModel>)
            };

            TypeInfo b = new TypeInfo()
            {
                GenericParametersInfo = CreateGenericParametersWithConstraints<TechnicalEmployee, TechnicalEmployeeViewModel>(),
                Type = typeof(TechnicalEmployeeMappingHandler<,>)
            };

            IHandlerSearch search = new HandlerSearch();

            Assert.That(search.Compare(a, b), Is.False);
        }

        [Test]
        public void Compare_TypeInfoAndTypeInfo_GenericParameterConstraintIsGenericTypeWhichIsConstructedMatch()
        {
            TypeInfo a = new TypeInfo()
            {
                GenericParametersInfo = CreateGenericParameter<MapperPayload<Employee, EmployeeViewModel>>(),
                Type = typeof(EmployeePayloadMappingHandler<MapperPayload<Employee, EmployeeViewModel>>)
            };

            TypeInfo b = new TypeInfo()
            {
                GenericParametersInfo = CreateGenericParameterWithConstraints<MapperPayload<Employee, EmployeeViewModel>>(),
                Type = typeof(EmployeePayloadMappingHandler<>)
            };

            IHandlerSearch search = new HandlerSearch();

            Assert.That(search.Compare(a, b), Is.True);
        }

        [Test]
        public void Compare_TypeInfoAndTypeInfo_GenericParameterConstraintIsGenericTypeWhichIsConstructedNoMatch()
        {
            TypeInfo a = new TypeInfo()
            {
                GenericParametersInfo = CreateGenericParameter<MapperPayload<Employee, EmployeeViewModel>>(),
                Type = typeof(EmployeePayloadMappingHandler<MapperPayload<Employee, EmployeeViewModel>>)
            };

            TypeInfo b = new TypeInfo()
            {
                GenericParametersInfo = CreateGenericParameterWithConstraints<MapperPayload<Developer, DeveloperViewModel>>(),
                Type = typeof(EmployeePayloadMappingHandler<>)
            };

            IHandlerSearch search = new HandlerSearch();

            Assert.That(search.Compare(a, b), Is.False);
        }

        [Test]
        public void Compare_TypeInfoAndTypeInfo_GenericParameterConstraintIsGenericTypeWhichIsNotConstructedMatch()
        {
            TypeInfo a = new TypeInfo()
            {
                GenericParametersInfo = CreateGenericParameters<MapperPayload<Developer, DeveloperViewModel>, Developer, DeveloperViewModel>(),
                Type = typeof(EmployeePayloadMappingHandler<MapperPayload<Developer, DeveloperViewModel>, Developer, DeveloperViewModel>)
            };

            TypeInfo b = new TypeInfo()
            {
                GenericParametersInfo = CreateGenericParametersWithConstraints(typeof(MapperPayload<,>), typeof(Employee), typeof(EmployeeViewModel)),
                Type = typeof(EmployeePayloadMappingHandler<,,>)
            };

            IHandlerSearch search = new HandlerSearch();

            Assert.That(search.Compare(a, b), Is.True);
        }

        [Test]
        public void Compare_TypeInfoAndTypeInfo_GenericParameterConstraintIsGenericTypeWhichIsNotConstructedNoMatch()
        {
            TypeInfo a = new TypeInfo()
            {
                GenericParametersInfo = CreateGenericParameters<Payload<Developer, DeveloperViewModel>, Developer, DeveloperViewModel>(),
                Type = typeof(EmployeePayloadHandler<Payload<Developer>, Developer, DeveloperViewModel>)
            };

            TypeInfo b = new TypeInfo()
            {
                GenericParametersInfo = CreateGenericParametersWithConstraints(typeof(MapperPayload<,>), typeof(Developer), typeof(DeveloperViewModel)),
                Type = typeof(EmployeePayloadMappingHandler<,,>)
            };

            IHandlerSearch search = new HandlerSearch();

            Assert.That(search.Compare(a, b), Is.False);
        }

        #region // Helpers

        private IList<GenericParameterInfo> CreateGenericParameterWithConstraints<TParam1>()
        {
            return new List<GenericParameterInfo>()
            {
                new GenericParameterInfo() { FilteredContraints = CreateTypesInfo<TParam1>() }
            };
        }

        private IList<GenericParameterInfo> CreateGenericParameter<TParam1>()
        {
            return new List<GenericParameterInfo>()
            {
                 CreateTypeInfo<GenericParameterInfo>(typeof(TParam1))
            };
        }

        private IList<GenericParameterInfo> CreateGenericParameters<TParam1, TParam2>()
        {
            return new List<GenericParameterInfo>()
            {
                 CreateTypeInfo<GenericParameterInfo>(typeof(TParam1)),
                 CreateTypeInfo<GenericParameterInfo>(typeof(TParam2))
            };
        }

        private IList<GenericParameterInfo> CreateGenericParameters<TParam1, TParam2, TParam3>()
        {
            return new List<GenericParameterInfo>()
            {
                 CreateTypeInfo<GenericParameterInfo>(typeof(TParam1)),
                 CreateTypeInfo<GenericParameterInfo>(typeof(TParam2)),
                 CreateTypeInfo<GenericParameterInfo>(typeof(TParam3)),
            };
        }

        private IList<GenericParameterInfo> CreateGenericParameters(Type type1, Type type2, Type type3)
        {
            List<GenericParameterInfo> toReturn = new List<GenericParameterInfo>();

            if (type1 != null)
                toReturn.Add(CreateTypeInfo<GenericParameterInfo>(type1));
            if (type2 != null)
                toReturn.Add(CreateTypeInfo<GenericParameterInfo>(type2));
            if (type3 != null)
                toReturn.Add(CreateTypeInfo<GenericParameterInfo>(type3));

            return toReturn;
        }

        private IList<GenericParameterInfo> CreateGenericParametersWithConstraints<TParam1, TParam2>()
        {
            return new List<GenericParameterInfo>()
            {
                new GenericParameterInfo() { FilteredContraints = CreateTypesInfo<TParam1>(), Type = typeof(TParam1) },
                new GenericParameterInfo() { FilteredContraints = CreateTypesInfo<TParam2>(), Type = typeof(TParam2) }
            };
        }

        private IList<GenericParameterInfo> CreateGenericParametersWithConstraints<TParam1, TParam2, TParam3>()
        {
            return new List<GenericParameterInfo>()
            {
                new GenericParameterInfo() { FilteredContraints = CreateTypesInfo<TParam1>(), Type = typeof(TParam1) },
                new GenericParameterInfo() { FilteredContraints = CreateTypesInfo<TParam2>(), Type = typeof(TParam2) },
                new GenericParameterInfo() { FilteredContraints = CreateTypesInfo<TParam3>(), Type = typeof(TParam3) }
            };
        }

        private IList<GenericParameterInfo> CreateGenericParametersWithConstraints(Type type1, Type type2, Type type3)
        {
            List<GenericParameterInfo> toReturn = new List<GenericParameterInfo>();

            if (type1 != null)
                toReturn.Add(new GenericParameterInfo() { FilteredContraints = CreateTypesInfo(type1), Type = type1 });
            if (type2 != null)
                toReturn.Add(new GenericParameterInfo() { FilteredContraints = CreateTypesInfo(type2), Type = type2 });
            if (type3 != null)
                toReturn.Add(new GenericParameterInfo() { FilteredContraints = CreateTypesInfo(type3), Type = type3 });

            return toReturn;
        }

        private IList<TypeInfo> CreateTypesInfo<TParam1>()
        {
            return CreateTypesInfo(typeof(TParam1));
        }

        private IList<TypeInfo> CreateTypesInfo(Type type)
        {
            return new List<TypeInfo>() { CreateTypeInfo(type) };
        }

        private TypeInfo CreateTypeInfo(Type type)
        {
            return CreateTypeInfo<TypeInfo>(type);
        }

        private TTypeInfo CreateTypeInfo<TTypeInfo>(Type type)
            where TTypeInfo : TypeInfo, new()
        {
            return new TTypeInfo()
            {
                Type = type,
                IsConstructed = type.IsGenericType && type.IsConstructedGenericType,
                IsGenericType = type.IsGenericType,
                GenericTypeDefinition = type.IsGenericType ? type.GetGenericTypeDefinition() : null
            };
        }

        #endregion // Helpers
    }
}