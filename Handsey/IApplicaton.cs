﻿using Handsey.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey
{
    internal interface IApplicaton
    {
        void Init();

        void Invoke<THandle>()
            where THandle : IHandles;
    }
}