using Handsey.Tests.Integration.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey.Tests.Integration.Models
{
    public class TechnicalArchitect : TechnicalEmployee
    {
        public TechnicalArchitect(string firstName
            , string lastName
            , string[] programmingLanguages)
            : base(firstName, lastName, programmingLanguages)
        { }

        protected override void FireChange()
        {
            Application.Invoke<IChangeHandler<TechnicalArchitect>>(h => h.Handle(this));
        }
    }
}