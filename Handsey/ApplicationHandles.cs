﻿using Handsey.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey
{
    public class ApplicationHandles
    {
        private readonly IList<TypeInfo> _classes;

        public ApplicationHandles(IList<TypeInfo> classes)
        {
            _classes = classes;
        }

        public IList<TypeInfo> Find<THandler>(IHandlerSearch search)
            where THandler : IHandles
        {
            // this is going to be a double dispatch method :)
            // search.Execute<THandler>(_classes);
            throw new NotImplementedException("TODO");
        }
    }
}