﻿using Handsey.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey
{
    public interface IHandlerSearch
    {
        IList<ClassInfo> Execute<THandler>(IList<ClassInfo> listToSearch);
    }
}