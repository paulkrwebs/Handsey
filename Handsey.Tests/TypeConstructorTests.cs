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
    public class TypeConstructorTests
    {
        [Test]
        public void Create_TypeInfoAndTypeInfoAsList_NullPassedInSoArgumentExceptionThrown()
        {
            ITypeConstructor factory = new TypeConstructor();

            Assert.That(() => factory.Create(new TypeInfo(), null as IList<TypeInfo>).ToList(), Throws.Exception.TypeOf<ArgumentNullException>());
            Assert.That(() => factory.Create(null, new List<TypeInfo>()).ToList(), Throws.Exception.TypeOf<ArgumentNullException>());
            Assert.That(() => factory.Create(new TypeInfo(), new List<TypeInfo>() { new TypeInfo() }).ToList(), Throws.Exception.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Create_TypeInfoAndTypeInfoAsList_AllTypesInListConstructed()
        {
            TypeInfo constructedFrom = new TypeInfo()
            {
                GenericParametersInfo = CreateGenericParameters<Developer, DeveloperViewModel>()
            };

            TypeInfo typeToBeConstructed1 = new TypeInfo()
            {
                IsConstructed = false,
                IsGenericType = true,
                Type = typeof(EmployeeMappingHandler<,>),
                GenericParametersInfo = CreateGenericParametersWithConstraints<Employee, EmployeeViewModel>()
            };

            TypeInfo typeToBeConstructed2 = new TypeInfo()
            {
                IsConstructed = false,
                IsGenericType = true,
                Type = typeof(DeveloperMappingHandler<,>),
                GenericParametersInfo = CreateGenericParametersWithConstraints<Developer, DeveloperViewModel>()
            };

            ITypeConstructor factory = new TypeConstructor();
            IList<Type> constructedTypes = factory.Create(constructedFrom, new List<TypeInfo>() { typeToBeConstructed1, typeToBeConstructed2 }).ToList();

            Assert.That(constructedTypes[0], Is.EqualTo(typeof(EmployeeMappingHandler<Developer, DeveloperViewModel>)));
            Assert.That(constructedTypes[1], Is.EqualTo(typeof(DeveloperMappingHandler<Developer, DeveloperViewModel>)));
        }

        [Test]
        public void Create_TypeInfoAndTypeInfo_NullPassedInSoArgumentExceptionThrown()
        {
            ITypeConstructor factory = new TypeConstructor();

            Assert.That(() => factory.Create(new TypeInfo(), null as TypeInfo), Throws.Exception.TypeOf<ArgumentNullException>());
            Assert.That(() => factory.Create(null, new TypeInfo()), Throws.Exception.TypeOf<ArgumentNullException>());
            Assert.That(() => factory.Create(new TypeInfo(), new TypeInfo()), Throws.Exception.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Create_TypeInfoAndTypeInfo_ConcreteTypeSoConcreteTypeReturned()
        {
            TypeInfo typeToBeConstructed = new TypeInfo()
            {
                IsConstructed = true,
                IsGenericType = false,
                Type = typeof(TechnicalArchitectMappingHandler)
            };

            ITypeConstructor factory = new TypeConstructor();

            Assert.That(factory.Create(new TypeInfo(), typeToBeConstructed), Is.EqualTo(typeToBeConstructed.Type));
        }

        [Test]
        public void Create_TypeInfoAndTypeInfo_GenericTypeButConstructedFromDoesNotHAveGenericParametersSoExceptionThrown()
        {
            TypeInfo typeToBeConstructed = new TypeInfo()
            {
                IsConstructed = false,
                IsGenericType = true,
                Type = typeof(TechnicalArchitectMappingHandler)
            };

            ITypeConstructor factory = new TypeConstructor();

            Assert.That(() => factory.Create(new TypeInfo(), typeToBeConstructed), Throws.Exception.TypeOf<ArgumentException>());
        }

        [Test]
        public void Create_TypeInfoAndTypeInfo_ConstructedFromDoesNotHaveCorrectNumberOFGenericParametersSoExceptionThrown()
        {
            TypeInfo constructedFrom = new TypeInfo()
            {
                GenericParametersInfo = new GenericParameterInfo[1] { new GenericParameterInfo() },
            };

            TypeInfo typeToBeConstructed = new TypeInfo()
            {
                IsConstructed = false,
                IsGenericType = true,
                Type = typeof(TechnicalArchitectMappingHandler),
                GenericParametersInfo = new GenericParameterInfo[0],
            };

            ITypeConstructor factory = new TypeConstructor();

            Assert.That(() => factory.Create(constructedFrom, typeToBeConstructed), Throws.Exception.TypeOf<ArgumentException>());
        }

        [Test]
        public void Create_TypeInfoAndTypeInfo_GenericTypeConstructed()
        {
            TypeInfo constructedFrom = new TypeInfo()
            {
                GenericParametersInfo = CreateGenericParameters<Payload<Developer, DeveloperViewModel>, Developer, DeveloperViewModel>()
            };

            TypeInfo typeToBeConstructed = new TypeInfo()
            {
                IsConstructed = false,
                IsGenericType = true,
                Type = typeof(EmployeePayloadHandler<,,>),
                GenericParametersInfo = CreateGenericParametersWithConstraints(typeof(Payload<,>), typeof(Developer), typeof(DeveloperViewModel))
            };

            ITypeConstructor factory = new TypeConstructor();

            Assert.That(factory.Create(constructedFrom, typeToBeConstructed), Is.EqualTo(typeof(EmployeePayloadHandler<Payload<Developer, DeveloperViewModel>, Developer, DeveloperViewModel>)));
        }

        #region // Helpers MOVE TO UTILITIES

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

        #endregion // Helpers MOVE TO UTILITIES
    }
}