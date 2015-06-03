using Handsey.Tests.Integration.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey.Tests.Integration.Models
{
    public class Developer : TechnicalEmployee
    {
        public Developer(string firstName
            , string lastName
            , string[] programmingLanguages)
            : base(firstName
                    , lastName
                    , programmingLanguages)
        { }

        protected override void FireChange()
        {
            Application.Invoke<IChangeHandler<Developer>>(h => h.Handle(this));
        }
    }
}