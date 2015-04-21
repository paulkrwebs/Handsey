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
    public class EmployeePayloadHandler<TPayload, TData1> : IHandler<TPayload>
        where TPayload : Payload<TData1>
        where TData1 : Employee
    {
        public void Handle(TPayload arg1)
        {
            throw new NotImplementedException();
        }
    }

    public class EmployeePayloadHandler<TPayload, TData1, TData2> : IHandler<TPayload>
        where TPayload : Payload<TData1, TData2>
        where TData1 : Employee
        where TData2 : EmployeeViewModel
    {
        public void Handle(TPayload arg1)
        {
            throw new NotImplementedException();
        }
    }
}