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
    public class HandlerSearchTests
    {
        [Test]
        public void Execute_HandlerInfoAndListOfHandlerInfo_ArgumentsNullSoEmptyListReturned()
        {
            IHandlerSearch search = new HandlerSearch();
            IList<HandlerInfo> results = search.Execute(null, null).ToList();

            Assert.That(results.Count, Is.EqualTo(0));
        }

        [Test]
        public void Execute_HandlerInfoAndListOfHandlerInfo_CorrectHandlerMAtchedInList()
        {
            HandlerInfo a = new HandlerInfo()
            {
                GenericParametersInfo = CreateGenericParameters<Payload<Developer, DeveloperViewModel>, Developer, DeveloperViewModel>(),
                Type = typeof(EmployeePayloadHandler<Payload<Developer, DeveloperViewModel>, Developer, DeveloperViewModel>)
            };

            HandlerInfo notMatched1 = new HandlerInfo()
            {
                GenericParametersInfo = CreateGenericParametersWithConstraints(typeof(MapperPayload<,>), typeof(Developer), typeof(DeveloperViewModel)),
                Type = typeof(EmployeePayloadMappingHandler<,,>)
            };

            HandlerInfo matched1 = new HandlerInfo()
            {
                GenericParametersInfo = CreateGenericParametersWithConstraints(typeof(Payload<,>), typeof(Developer), typeof(DeveloperViewModel)),
                Type = typeof(EmployeePayloadMappingHandler<,,>)
            };

            HandlerInfo matched2 = new HandlerInfo()
            {
                GenericParametersInfo = CreateGenericParametersWithConstraints(typeof(Payload<,>), typeof(Employee), typeof(EmployeeViewModel)),
                Type = typeof(EmployeePayloadMappingHandler<,,>)
            };

            IHandlerSearch search = new HandlerSearch();
            IList<HandlerInfo> results = search.Execute(a, new List<HandlerInfo>() { notMatched1, matched1, matched2 }).ToList();

            Assert.That(results.Count, Is.EqualTo(2));
            Assert.That(results[0], Is.EqualTo(matched1));
            Assert.That(results[1], Is.EqualTo(matched2));
        }

        [Test]
        public void Execute_HandlerInfoAndListOfHandlerInfo_NoHandlersMatchedSoEmptyListReturned()
        {
            HandlerInfo a = new HandlerInfo()
            {
                GenericParametersInfo = CreateGenericParameter<Developer>(),
                Type = typeof(EmployeeHandler<Developer>)
            };

            HandlerInfo notMatched1 = new HandlerInfo()
            {
                GenericParametersInfo = CreateGenericParametersWithConstraints(typeof(MapperPayload<,>), typeof(Developer), typeof(DeveloperViewModel)),
                Type = typeof(EmployeePayloadMappingHandler<,,>)
            };

            IHandlerSearch search = new HandlerSearch();
            IList<HandlerInfo> results = search.Execute(a, new List<HandlerInfo>() { notMatched1 }).ToList();

            Assert.That(results.Count, Is.EqualTo(0));
        }

        [Test]
        public void Compare_HandlerInfoAndHandlerInfo_NullCheck()
        {
            HandlerInfo a = new HandlerInfo()
            {
                Type = typeof(TechnicalArchitectMappingHandler)
            };

            HandlerInfo b = new HandlerInfo()
            {
                Type = typeof(TechnicalArchitectMappingHandler)
            };

            IHandlerSearch search = new HandlerSearch();

            Assert.That(search.Compare(null, b), Is.False);
            Assert.That(search.Compare(a, null), Is.False);
            Assert.That(search.Compare(new HandlerInfo(), b), Is.False);
            Assert.That(search.Compare(a, new HandlerInfo()), Is.False);
        }

        [Test]
        public void Compare_HandlerInfoAndHandlerInfo_ExactTypeMatch()
        {
            HandlerInfo a = new HandlerInfo()
            {
                Type = typeof(TechnicalArchitectMappingHandler)
            };

            HandlerInfo b = new HandlerInfo()
            {
                Type = typeof(TechnicalArchitectMappingHandler)
            };

            IHandlerSearch search = new HandlerSearch();

            Assert.That(search.Compare(a, b), Is.True);
        }

        [Test]
        public void Compare_HandlerInfoAndHandlerInfo_ExactTypeNoMatch()
        {
            HandlerInfo a = new HandlerInfo()
            {
                Type = typeof(TechnicalArchitectMappingHandler)
            };

            HandlerInfo b = new HandlerInfo()
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
        public void Compare_HandlerInfoAndHandlerInfo_IsAssignableMatch(Type typeA, Type typeB)
        {
            HandlerInfo a = new HandlerInfo()
            {
                Type = typeA
            };

            HandlerInfo b = new HandlerInfo()
            {
                Type = typeB
            };

            IHandlerSearch search = new HandlerSearch();

            Assert.That(search.Compare(a, b), Is.True);
        }

        [Test]
        public void Compare_HandlerInfoAndHandlerInfo_IsAssignableNoMatch()
        {
            HandlerInfo a = new HandlerInfo()
            {
                Type = typeof(IOneToOneDataPopulation<TechnicalArchitect, TechnicalArchitectViewModel>)
            };

            HandlerInfo b = new HandlerInfo()
            {
                Type = typeof(EmployeeHandler)
            };

            IHandlerSearch search = new HandlerSearch();

            Assert.That(search.Compare(a, b), Is.False);
        }

        [Test]
        public void Compare_HandlerInfoAndHandlerInfo_GenericParameterAssingableMatch()
        {
            HandlerInfo a = new HandlerInfo()
            {
                GenericParametersInfo = CreateGenericParameters<Developer, DeveloperViewModel>(),
                Type = typeof(EmployeeMappingHandler<Developer, DeveloperViewModel>)
            };

            HandlerInfo b = new HandlerInfo()
            {
                GenericParametersInfo = CreateGenericParameters<Developer, DeveloperViewModel>(),
                Type = typeof(EmployeeMappingHandler<Employee, EmployeeViewModel>)
            };

            IHandlerSearch search = new HandlerSearch();

            Assert.That(search.Compare(a, b), Is.True);
        }

        [Test]
        public void Compare_HandlerInfoAndHandlerInfo_GenericParameterAssingableNoMatch()
        {
            HandlerInfo a = new HandlerInfo()
            {
                GenericParametersInfo = CreateGenericParameters<Developer, DeveloperViewModel>(),
                Type = typeof(EmployeeMappingHandler<Developer, DeveloperViewModel>)
            };

            HandlerInfo b = new HandlerInfo()
            {
                GenericParametersInfo = CreateGenericParameters<TechnicalArchitect, TechnicalArchitectViewModel>(),
                Type = typeof(EmployeeMappingHandler<Employee, EmployeeViewModel>)
            };

            IHandlerSearch search = new HandlerSearch();

            Assert.That(search.Compare(a, b), Is.False);
        }

        [Test]
        public void Compare_HandlerInfoAndHandlerInfo_GenericParameterConstraintAssingableMatch()
        {
            HandlerInfo a = new HandlerInfo()
            {
                GenericParametersInfo = CreateGenericParameters<Developer, DeveloperViewModel>(),
                Type = typeof(EmployeeMappingHandler<Developer, DeveloperViewModel>)
            };

            HandlerInfo b = new HandlerInfo()
            {
                GenericParametersInfo = CreateGenericParametersWithConstraints<Employee, EmployeeViewModel>(),
                Type = typeof(EmployeeMappingHandler<,>)
            };

            IHandlerSearch search = new HandlerSearch();

            Assert.That(search.Compare(a, b), Is.True);
        }

        [Test]
        public void Compare_HandlerInfoAndHandlerInfo_GenericParameterConstraintAssingableNoMatch()
        {
            HandlerInfo a = new HandlerInfo()
            {
                GenericParametersInfo = CreateGenericParameters<Employee, EmployeeViewModel>(),
                Type = typeof(EmployeeMappingHandler<Employee, EmployeeViewModel>)
            };

            HandlerInfo b = new HandlerInfo()
            {
                GenericParametersInfo = CreateGenericParametersWithConstraints<TechnicalEmployee, TechnicalEmployeeViewModel>(),
                Type = typeof(TechnicalEmployeeMappingHandler<,>)
            };

            IHandlerSearch search = new HandlerSearch();

            Assert.That(search.Compare(a, b), Is.False);
        }

        [Test]
        public void Compare_HandlerInfoAndHandlerInfo_GenericParameterMultipleConstraintAssingableMatch()
        {
            HandlerInfo a = new HandlerInfo()
            {
                GenericParametersInfo = CreateGenericParameter<VersionableFooModel>(),
                Type = typeof(VersionableFooHandler<VersionableFooModel>)
            };

            HandlerInfo b = new HandlerInfo()
            {
                GenericParametersInfo = CreateGenericParameterWithConstraints<IVersionable, IFooModel>(),
                Type = typeof(VersionableFooHandler<>)
            };

            IHandlerSearch search = new HandlerSearch();

            Assert.That(search.Compare(a, b), Is.True);
        }

        [Test]
        public void Compare_HandlerInfoAndHandlerInfo_GenericParameterMultipleConstraintAssingableNoMatch()
        {
            HandlerInfo a = new HandlerInfo()
            {
                GenericParametersInfo = CreateGenericParameter<VersionableFooModel>(),
                Type = typeof(VersionableFooHandler<VersionableFooModel>)
            };

            HandlerInfo b = new HandlerInfo()
            {
                GenericParametersInfo = CreateGenericParameterWithConstraints<IVersionable, Employee>(),
                Type = typeof(VersionableFooHandler<>)
            };

            IHandlerSearch search = new HandlerSearch();

            Assert.That(search.Compare(a, b), Is.False);
        }

        [Test]
        public void Compare_HandlerInfoAndHandlerInfo_GenericParameterSpecialConstraintRequiresReferenceTypeMatch()
        {
            HandlerInfo a = new HandlerInfo()
            {
                GenericParametersInfo = CreateGenericParameter<VersionableValueType>(),
                Type = typeof(VersioableValueHandler<VersionableValueType>)
            };

            a.GenericParametersInfo[0].IsValueType = false;

            HandlerInfo b = new HandlerInfo()
            {
                GenericParametersInfo = CreateGenericParametersWithConstraint<IVersionable>(GenericParameterAttributes.ReferenceTypeConstraint),
                Type = typeof(VersioableValueHandler<>)
            };

            IHandlerSearch search = new HandlerSearch();

            Assert.That(search.Compare(a, b), Is.True);
        }

        [Test]
        public void Compare_HandlerInfoAndHandlerInfo_GenericParameterSpecialConstraintRequiresReferenceTypeNoMatch()
        {
            HandlerInfo a = new HandlerInfo()
            {
                GenericParametersInfo = CreateGenericParameter<VersionableValueType>(),
                Type = typeof(VersioableValueHandler<VersionableValueType>)
            };

            a.GenericParametersInfo[0].IsValueType = true;

            HandlerInfo b = new HandlerInfo()
            {
                GenericParametersInfo = CreateGenericParametersWithConstraint<IVersionable>(GenericParameterAttributes.ReferenceTypeConstraint),
                Type = typeof(VersioableValueHandler<>)
            };

            IHandlerSearch search = new HandlerSearch();

            Assert.That(search.Compare(a, b), Is.False);
        }

        [Test]
        public void Compare_HandlerInfoAndHandlerInfo_GenericParameterSpecialConstraintRequiresDefaultConstructorMatch()
        {
            HandlerInfo a = new HandlerInfo()
            {
                GenericParametersInfo = CreateGenericParameter<VersionableValueType>(),
                Type = typeof(VersioableValueHandler<VersionableValueType>)
            };

            a.GenericParametersInfo[0].HasDefaultConstuctor = true;

            HandlerInfo b = new HandlerInfo()
            {
                GenericParametersInfo = CreateGenericParametersWithConstraint<IVersionable>(GenericParameterAttributes.DefaultConstructorConstraint),
                Type = typeof(VersioableValueHandler<>)
            };

            IHandlerSearch search = new HandlerSearch();

            Assert.That(search.Compare(a, b), Is.True);
        }

        [Test]
        public void Compare_HandlerInfoAndHandlerInfo_GenericParameterSpecialConstraintRequiresDefaultConstructorNoMatch()
        {
            HandlerInfo a = new HandlerInfo()
            {
                GenericParametersInfo = CreateGenericParameter<VersionableValueType>(),
                Type = typeof(VersioableValueHandler<VersionableValueType>)
            };

            a.GenericParametersInfo[0].HasDefaultConstuctor = false;

            HandlerInfo b = new HandlerInfo()
            {
                GenericParametersInfo = CreateGenericParametersWithConstraint<IVersionable>(GenericParameterAttributes.DefaultConstructorConstraint),
                Type = typeof(VersioableValueHandler<>)
            };

            IHandlerSearch search = new HandlerSearch();

            Assert.That(search.Compare(a, b), Is.False);
        }

        [Test]
        public void Compare_HandlerInfoAndHandlerInfo_GenericParameterConstraintIsGenericTypeWhichIsConstructedMatch()
        {
            HandlerInfo a = new HandlerInfo()
            {
                GenericParametersInfo = CreateGenericParameter<MapperPayload<Employee, EmployeeViewModel>>(),
                Type = typeof(EmployeePayloadMappingHandler<MapperPayload<Employee, EmployeeViewModel>>)
            };

            HandlerInfo b = new HandlerInfo()
            {
                GenericParametersInfo = CreateGenericParameterWithConstraint<MapperPayload<Employee, EmployeeViewModel>>(),
                Type = typeof(EmployeePayloadMappingHandler<>)
            };

            IHandlerSearch search = new HandlerSearch();

            Assert.That(search.Compare(a, b), Is.True);
        }

        [Test]
        public void Compare_HandlerInfoAndHandlerInfo_GenericParameterConstraintIsGenericTypeWhichIsConstructedNoMatch()
        {
            HandlerInfo a = new HandlerInfo()
            {
                GenericParametersInfo = CreateGenericParameter<MapperPayload<Employee, EmployeeViewModel>>(),
                Type = typeof(EmployeePayloadMappingHandler<MapperPayload<Employee, EmployeeViewModel>>)
            };

            HandlerInfo b = new HandlerInfo()
            {
                GenericParametersInfo = CreateGenericParameterWithConstraint<MapperPayload<Developer, DeveloperViewModel>>(),
                Type = typeof(EmployeePayloadMappingHandler<>)
            };

            IHandlerSearch search = new HandlerSearch();

            Assert.That(search.Compare(a, b), Is.False);
        }

        [Test]
        public void Compare_HandlerInfoAndHandlerInfo_GenericParameterConstraintIsGenericTypeWhichIsNotConstructedMatch()
        {
            HandlerInfo a = new HandlerInfo()
            {
                GenericParametersInfo = CreateGenericParameters<MapperPayload<Developer, DeveloperViewModel>, Developer, DeveloperViewModel>(),
                Type = typeof(EmployeePayloadMappingHandler<MapperPayload<Developer, DeveloperViewModel>, Developer, DeveloperViewModel>)
            };

            HandlerInfo b = new HandlerInfo()
            {
                GenericParametersInfo = CreateGenericParametersWithConstraints(typeof(MapperPayload<,>), typeof(Employee), typeof(EmployeeViewModel)),
                Type = typeof(EmployeePayloadMappingHandler<,,>)
            };

            IHandlerSearch search = new HandlerSearch();

            Assert.That(search.Compare(a, b), Is.True);
        }

        [Test]
        public void Compare_HandlerInfoAndHandlerInfo_GenericParameterWithNestedGenericParametersMatch()
        {
            HandlerInfo a = new HandlerInfo()
            {
                GenericParametersInfo = CreateGenericParameter<MapperPayload<Employee, EmployeeViewModel>>(),
                Type = typeof(IHandler<MapperPayload<Developer, DeveloperViewModel>>),
                ConcreteNestedGenericParametersInfo = new Dictionary<string, GenericParameterInfo>()
                {
                    { "Developer" , new GenericParameterInfo() { Type = typeof(Developer) , Position = 0} },
                    { "DeveloperViewModel" , new GenericParameterInfo() { Type = typeof(DeveloperViewModel), Position = 0  } }
                },
                IsInterface = true,
                IsGenericType = true,
                GenericTypeDefinition = typeof(IHandler<>)
            };
            a.GenericParametersInfo[0].GenericParametersInfo = CreateGenericParameters<Employee, EmployeeViewModel>();

            HandlerInfo b = new HandlerInfo()
            {
                GenericParametersInfo = CreateGenericParametersWithConstraints(typeof(Employee), typeof(EmployeeViewModel)),
                Type = typeof(EmployeePayloadMappingHandler<,>),
                ConcreteNestedGenericParametersInfo = new Dictionary<string, GenericParameterInfo>()
                {
                    { "TFrom" , new GenericParameterInfo() { FilteredContraints = CreateTypeInfo<Employee>(), Position = 0 } },
                    { "TTo" , new GenericParameterInfo() { FilteredContraints = CreateTypeInfo<EmployeeViewModel>(), Position = 1 } }
                },
                FilteredInterfaces = new HandlerInfo[1]
                {
                    new HandlerInfo()
                    {
                        Type = typeof(IHandler<>),
                        IsGenericType = true,
                        GenericTypeDefinition = typeof(IHandler<>),
                        ConcreteNestedGenericParametersInfo = new Dictionary<string, GenericParameterInfo>()
                        {
                            { "TTo" , new GenericParameterInfo() { Position = 0 } },
                            { "TFrom" , new GenericParameterInfo() {Position = 1 } },
                        }
                    }
                }
            };

            IHandlerSearch search = new HandlerSearch();

            Assert.That(search.Compare(a, b), Is.True);
        }

        [Test]
        public void Compare_HandlerInfoAndHandlerInfo_GenericParameterConstraintIsGenericTypeWhichIsNotConstructedNoMatch()
        {
            HandlerInfo a = new HandlerInfo()
            {
                GenericParametersInfo = CreateGenericParameter<MapperPayload<Employee, EmployeeViewModel>>(),
                Type = typeof(IHandler<MapperPayload<Developer, DeveloperViewModel>>),
                ConcreteNestedGenericParametersInfo = new Dictionary<string, GenericParameterInfo>()
                {
                    { "Developer" , new GenericParameterInfo() { Type = typeof(Developer) , Position = 0} },
                    { "DeveloperViewModel" , new GenericParameterInfo() { Type = typeof(DeveloperViewModel), Position = 0  } }
                },
                IsInterface = true,
                IsGenericType = true,
                GenericTypeDefinition = typeof(IHandler<>)
            };
            a.GenericParametersInfo[0].GenericParametersInfo = CreateGenericParameters<Employee, EmployeeViewModel>();

            HandlerInfo b = new HandlerInfo()
            {
                GenericParametersInfo = CreateGenericParametersWithConstraints(typeof(TechnicalArchitect), typeof(TechnicalArchitectViewModel)),
                Type = typeof(EmployeePayloadMappingHandler<,>),
                ConcreteNestedGenericParametersInfo = new Dictionary<string, GenericParameterInfo>()
                {
                    { "TFrom" , new GenericParameterInfo() { FilteredContraints = CreateTypeInfo<TechnicalArchitect>(), Position = 0 } },
                    { "TTo" , new GenericParameterInfo() { FilteredContraints = CreateTypeInfo<TechnicalArchitectViewModel>(), Position = 1 } }
                },
                FilteredInterfaces = new HandlerInfo[1]
                {
                    new HandlerInfo()
                    {
                        Type = typeof(IHandler<>),
                        IsGenericType = true,
                        GenericTypeDefinition = typeof(IHandler<>),
                        ConcreteNestedGenericParametersInfo = new Dictionary<string, GenericParameterInfo>()
                        {
                            { "TTo" , new GenericParameterInfo() { Position = 0 } },
                            { "TFrom" , new GenericParameterInfo() {Position = 1 } },
                        }
                    }
                }
            };

            IHandlerSearch search = new HandlerSearch();

            Assert.That(search.Compare(a, b), Is.False);
        }

        [Test]
        public void Compare_HandlerInfoAndHandlerInfo_HandlerIsInterfaceAndHasDifferentNumberOfGenericParametersNoMatch()
        {
            HandlerInfo a = new HandlerInfo()
            {
                GenericParametersInfo = CreateGenericParameter<Employee>(),
                Type = typeof(IHandler<Developer>),
                ConcreteNestedGenericParametersInfo = new Dictionary<string, GenericParameterInfo>()
                {
                    { "Developer" , new GenericParameterInfo() { Type = typeof(Developer) , Position = 0} },
                },
                IsInterface = true,
                IsGenericType = true,
                GenericTypeDefinition = typeof(IHandler<>)
            };
            a.GenericParametersInfo[0].GenericParametersInfo = CreateGenericParameters<Employee, EmployeeViewModel>();

            HandlerInfo b = new HandlerInfo()
            {
                GenericParametersInfo = CreateGenericParametersWithConstraints(typeof(TechnicalArchitect), typeof(TechnicalArchitectViewModel)),
                Type = typeof(EmployeePayloadMappingHandler<,>),
                ConcreteNestedGenericParametersInfo = new Dictionary<string, GenericParameterInfo>()
                {
                    { "TFrom" , new GenericParameterInfo() { FilteredContraints = CreateTypeInfo<TechnicalArchitect>(), Position = 0 } },
                    { "TTo" , new GenericParameterInfo() { FilteredContraints = CreateTypeInfo<TechnicalArchitectViewModel>(), Position = 1 } }
                },
                FilteredInterfaces = new HandlerInfo[1]
                {
                    new HandlerInfo()
                    {
                        Type = typeof(IHandler<>),
                        IsGenericType = true,
                        GenericTypeDefinition = typeof(IHandler<>),
                        ConcreteNestedGenericParametersInfo = new Dictionary<string, GenericParameterInfo>()
                        {
                            { "TTo" , new GenericParameterInfo() { Position = 0 } },
                            { "TFrom" , new GenericParameterInfo() {Position = 1 } },
                        }
                    }
                }
            };

            IHandlerSearch search = new HandlerSearch();

            Assert.That(search.Compare(a, b), Is.False);
        }

        #region // Helpers MOVE TO UTILITIES

        private IList<GenericParameterInfo> CreateGenericParameterWithConstraint<TParam1>()
        {
            return new List<GenericParameterInfo>()
            {
                new GenericParameterInfo() { FilteredContraints = CreateTypeInfo<TParam1>() }
            };
        }

        private IList<GenericParameterInfo> CreateGenericParameter<TParam1>()
        {
            return new List<GenericParameterInfo>()
            {
                 CreateHandlerInfo<GenericParameterInfo>(typeof(TParam1))
            };
        }

        private IList<GenericParameterInfo> CreateGenericParameters<TParam1, TParam2>()
        {
            return new List<GenericParameterInfo>()
            {
                 CreateHandlerInfo<GenericParameterInfo>(typeof(TParam1)),
                 CreateHandlerInfo<GenericParameterInfo>(typeof(TParam2))
            };
        }

        private IList<GenericParameterInfo> CreateGenericParameters<TParam1, TParam2, TParam3>()
        {
            return new List<GenericParameterInfo>()
            {
                 CreateHandlerInfo<GenericParameterInfo>(typeof(TParam1)),
                 CreateHandlerInfo<GenericParameterInfo>(typeof(TParam2)),
                 CreateHandlerInfo<GenericParameterInfo>(typeof(TParam3)),
            };
        }

        private IList<GenericParameterInfo> CreateGenericParameters(Type type1, Type type2, Type type3)
        {
            List<GenericParameterInfo> toReturn = new List<GenericParameterInfo>();

            if (type1 != null)
                toReturn.Add(CreateHandlerInfo<GenericParameterInfo>(type1));
            if (type2 != null)
                toReturn.Add(CreateHandlerInfo<GenericParameterInfo>(type2));
            if (type3 != null)
                toReturn.Add(CreateHandlerInfo<GenericParameterInfo>(type3));

            return toReturn;
        }

        private IList<GenericParameterInfo> CreateGenericParametersWithConstraint<TParam1>(GenericParameterAttributes specialAttributes = GenericParameterAttributes.None)
        {
            return new List<GenericParameterInfo>()
            {
                new GenericParameterInfo() { FilteredContraints = CreateTypeInfo<TParam1>(), Type = typeof(TParam1), SpecialConstraint = specialAttributes}
            };
        }

        private IList<GenericParameterInfo> CreateGenericParametersWithConstraints<TParam1, TParam2>()
        {
            return new List<GenericParameterInfo>()
            {
                new GenericParameterInfo() { FilteredContraints = CreateTypeInfo<TParam1>(), Type = typeof(TParam1) },
                new GenericParameterInfo() { FilteredContraints = CreateTypeInfo<TParam2>(), Type = typeof(TParam2) }
            };
        }

        private IList<GenericParameterInfo> CreateGenericParameterWithConstraints<TParam1, TParam2>()
        {
            return new List<GenericParameterInfo>()
            {
                new GenericParameterInfo() { FilteredContraints = CreateTypesInfo<TParam1, TParam2>(), Type = typeof(TParam1) },
            };
        }

        private IList<GenericParameterInfo> CreateGenericParametersWithConstraints<TParam1, TParam2, TParam3>()
        {
            return new List<GenericParameterInfo>()
            {
                new GenericParameterInfo() { FilteredContraints = CreateTypeInfo<TParam1>(), Type = typeof(TParam1) },
                new GenericParameterInfo() { FilteredContraints = CreateTypeInfo<TParam2>(), Type = typeof(TParam2) },
                new GenericParameterInfo() { FilteredContraints = CreateTypeInfo<TParam3>(), Type = typeof(TParam3) }
            };
        }

        private IList<GenericParameterInfo> CreateGenericParametersWithConstraints(Type type1, Type type2 = null, Type type3 = null)
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

        private IList<TypeInfo> CreateTypeInfo<TParam1>()
        {
            return CreateTypesInfo(typeof(TParam1));
        }

        private IList<TypeInfo> CreateTypesInfo<TParam1, TParam2>()
        {
            return CreateTypesInfo(typeof(TParam1), typeof(TParam2));
        }

        private IList<TypeInfo> CreateTypesInfo(params Type[] type)
        {
            return type.Select(t => CreateHandlerInfo(t)).ToList();
        }

        private TypeInfo CreateHandlerInfo(Type type)
        {
            return CreateHandlerInfo<TypeInfo>(type);
        }

        private TTypeInfo CreateHandlerInfo<TTypeInfo>(Type type)
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

        public struct VersionableValueType : IVersionable
        { }

        public class VersioableValueHandler<TType> : IHandler<IVersionable>
        {
            public void Handle(IVersionable arg1)
            {
                throw new NotImplementedException();
            }
        }

        #endregion // Helpers MOVE TO UTILITIES
    }
}