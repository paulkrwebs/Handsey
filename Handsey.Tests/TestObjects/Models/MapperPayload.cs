using Handsey.Tests.TestObjects.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey.Tests.TestObjects.Models
{
    public class MapperPayload<TMapFrom, TMapTo>
        where TMapFrom : Employee
        where TMapTo : EmployeeViewModel
    {
        public TMapFrom From { get; set; }

        public TMapTo To { get; set; }
    }
}