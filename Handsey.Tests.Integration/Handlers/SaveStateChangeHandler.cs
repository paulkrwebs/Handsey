﻿using Handsey.Tests.Integration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey.Tests.Integration.Handlers
{
    public class SaveStateChangeHandler<TVersionable> : IChangeHandler<TVersionable>
        where TVersionable : IVersionable
    {
        private readonly IHandlerCallLog _handlerCallLog;

        public SaveStateChangeHandler(IHandlerCallLog handlerCallLog)
        {
            _handlerCallLog = handlerCallLog;
        }

        public void Handle(TVersionable arg1)
        {
            // saves the changed made to a persisted storage area
            _handlerCallLog.Log.Add(this.GetType());
        }
    }
}