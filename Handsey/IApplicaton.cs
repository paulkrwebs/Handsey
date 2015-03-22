using Handsey.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey
{
    interface IApplicaton
    {
        void Init();

        void Invoke<THandle>()
            where THandle : IHandles;

        void Invoke<THandle, TArgs1>()
            where THandle : IHandles<TArgs1>;

        void Invoke<THandle, TArgs1, TArgs2>()
            where THandle : IHandles<TArgs1, TArgs2>;

        void Invoke<THandle, TArgs1, TArgs2, TArgs3>()
            where THandle : IHandles<TArgs1, TArgs2, TArgs3>;
    }
}