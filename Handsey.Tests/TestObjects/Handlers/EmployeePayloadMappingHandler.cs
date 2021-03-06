﻿using Handsey.Handlers;
using Handsey.Tests.TestObjects.Models;
using Handsey.Tests.TestObjects.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey.Tests.TestObjects.Handlers
{
    public class EmployeePayloadMappingHandler<TPayLoad> : IHandles<TPayLoad>
        where TPayLoad : MapperPayload<Employee, EmployeeViewModel>
    {
        public void Handle(TPayLoad arg1)
        {
            throw new NotImplementedException();
        }
    }

    public class EmployeePayloadMappingHandler<TPayLoad, TFrom, TTo> : IHandles<TPayLoad>
        where TPayLoad : MapperPayload<TFrom, TTo>
        where TFrom : Employee
        where TTo : EmployeeViewModel
    {
        public void Handle(TPayLoad arg1)
        {
            throw new NotImplementedException();
        }
    }
}