using Handsey.Tests.TestObjects.Models;
using Handsey.Tests.TestObjects.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey.Tests.TestObjects.Handlers
{
    public class DeveloperMappingHandler<TFrom, TTo> : IOneToOneDataPopulation<TFrom, TTo>
    {
        public void Handle(TFrom from, TTo to)
        {
        }
    }
}