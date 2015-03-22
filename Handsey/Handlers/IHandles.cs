using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey.Handlers
{
    public interface IHandles
    {    }

    public interface IHandles<TArgs1> : IHandles
    {
        void Handle(TArgs1 arg1);
    }

    public interface IHandles<TArgs1, TArgs2> : IHandles
    {
        void Handle(TArgs1 arg1, TArgs2 args2);
    }

    public interface IHandles<TArgs1, TArgs2, TArgs3> : IHandles
    {
        void Handle(TArgs1 arg1, TArgs2 args2, TArgs3 args3);
    }
}
