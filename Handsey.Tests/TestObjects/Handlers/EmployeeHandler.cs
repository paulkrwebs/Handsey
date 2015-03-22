using Handsey.Handlers;
using Handsey.Tests.TestObjects.Models;
using Handsey.Tests.TestObjects.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey.Tests.TestObjects.Handlers
{
    public class EmployeeHandler : IHandles, IFoo
    {
    }

    public class EmployeeHandler<TEmployee> : IHandles<TEmployee>
    {
        public void Handle(TEmployee arg1)
        {
            throw new NotImplementedException();
        }
    }
}