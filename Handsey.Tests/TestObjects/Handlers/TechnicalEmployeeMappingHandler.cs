using Handsey.Tests.TestObjects.Models;
using Handsey.Tests.TestObjects.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey.Tests.TestObjects.Handlers
{
    public class TechnicalEmployeeMappingHandler<TFrom, TTo> : IOneToOneDataPopulation<TFrom, TTo>
        where TFrom : TechnicalEmployee
        where TTo : TechnicalEmployeeViewModel
    {
        public void Handle(TFrom from, TTo to)
        {
            to.TechnicalLevel = from.TechnicalLevel;
        }
    }
}