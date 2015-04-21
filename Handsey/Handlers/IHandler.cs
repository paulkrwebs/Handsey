using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey.Handlers
{
    public interface IHandler
    { }

    public interface IHandler<TArgs1> : IHandler
    {
        void Handle(TArgs1 arg1);
    }

    public interface IHandler<TArgs1, TArgs2> : IHandler
    {
        void Handle(TArgs1 arg1, TArgs2 args2);
    }

    public interface IHandler<TArgs1, TArgs2, TArgs3> : IHandler
    {
        void Handle(TArgs1 arg1, TArgs2 args2, TArgs3 args3);
    }
}