using Handsey.Tests.TestObjects.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey.Tests.TestObjects.Models
{
    public class MapperPayload<TFrom, TTo>
        where TFrom : Employee
        where TTo : EmployeeViewModel
    {
        public TFrom From { get; set; }

        public TTo To { get; set; }
    }
}