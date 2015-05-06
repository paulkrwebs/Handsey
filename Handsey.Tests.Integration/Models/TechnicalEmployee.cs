using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey.Tests.Integration.Models
{
    public class TechnicalEmployee : Employee
    {
        public string[] ProgrammingLanguages { get; private set; }

        public void Change(string firstName, string lastName, string[] programminglanguages)
        {
            LogChange("TeamSize", String.Join(",", ProgrammingLanguages), String.Join(",", programminglanguages));
            ProgrammingLanguages = programminglanguages;

            base.Change(firstName, lastName);
        }
    }
}