using Handsey.Tests.Integration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey.Tests.Integration.Handlers
{
    public class ProgrammingLanguagesChangedHandler<TTechnicalEmployee> : IChangeHandler<TTechnicalEmployee>
        where TTechnicalEmployee : TechnicalEmployee, IVersionable, IVerifiable
    {
        public ProgrammingLanguagesChangedHandler()
        { }

        public void Handle(TTechnicalEmployee arg1)
        {
            // Inform the business a programmer has new skillz! arg1.ProgrammingLanguages
            arg1.UpdateLog(this);
        }
    }
}