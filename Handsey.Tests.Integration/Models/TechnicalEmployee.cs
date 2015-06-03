using Handsey.Tests.Integration.Handlers;
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

        public TechnicalEmployee(string firstName
            , string lastName
            , string[] programmingLanguages)
            : base(firstName, lastName)
        {
            ProgrammingLanguages = programmingLanguages;
        }

        public void Change(string firstName, string lastName, string[] programminglanguages)
        {
            LogChange("ProgrammingLanguages", String.Join(",", ProgrammingLanguages), String.Join(",", programminglanguages));
            ProgrammingLanguages = programminglanguages;

            base.Change(firstName, lastName);
        }

        protected override void FireChange()
        {
            Application.Invoke<IChangeHandler<TechnicalEmployee>>(h => h.Handle(this));
        }
    }
}