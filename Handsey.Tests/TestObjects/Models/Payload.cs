using Handsey.Tests.TestObjects.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey.Tests.TestObjects.Models
{
    public class Payload<TType>
        where TType : Employee
    {
        public TType Data { get; set; }
    }

    public class Payload<TType1, TType2>
        where TType1 : Employee
        where TType2 : EmployeeViewModel
    {
        public TType1 Data1 { get; set; }

        public TType2 Data2 { get; set; }
    }
}