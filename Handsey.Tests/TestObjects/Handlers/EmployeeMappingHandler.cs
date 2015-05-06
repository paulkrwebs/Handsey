using Handsey.Attributes;
using Handsey.Tests.TestObjects.Models;
using Handsey.Tests.TestObjects.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey.Tests.TestObjects.Handlers
{
    [HandlesFirst]
    public class EmployeeMappingHandler<TFrom, TTo> : IOneToOneDataPopulation<TFrom, TTo>, IFooHandler
        where TFrom : Employee
        where TTo : EmployeeViewModel
    {
        public void Handle(TFrom from, TTo to)
        {
            to.FullName = from.Name;
        }
    }
}