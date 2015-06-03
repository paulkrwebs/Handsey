using Handsey.Tests.Integration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey.Tests.Integration.Handlers
{
    public class ProgrammingLanguagesChangedHandler<TTechnicalEmployee> : IChangeHandler<TTechnicalEmployee>
        where TTechnicalEmployee : TechnicalEmployee, IVersionable
    {
        private readonly IHandlerCallLog _handlerCallLog;

        public ProgrammingLanguagesChangedHandler(IHandlerCallLog handlerCallLog)
        {
            _handlerCallLog = handlerCallLog;
        }

        public void Handle(TTechnicalEmployee arg1)
        {
            // Inform the business a programmer has new skillz! arg1.ProgrammingLanguages
            _handlerCallLog.Log.Add(this.GetType());
        }
    }
}