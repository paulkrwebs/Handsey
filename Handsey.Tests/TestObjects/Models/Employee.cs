using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey.Tests.TestObjects.Models
{
    public class Employee : IVersionable
    {
        public string Name { get; set; }
    }
}