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
    [HandlesLast]
    public class TechnicalArchitectMappingHandler : IOneToOneDataPopulation<TechnicalArchitect, TechnicalArchitectViewModel>
    {
        public void Handle(TechnicalArchitect from, TechnicalArchitectViewModel to)
        {
            to.CanUml = from.CanUml;
        }
    }
}