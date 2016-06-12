using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey.Handlers
{
    public interface IHandler<TArgs1, TArgs2> : IHandler
    {
        void Handle(TArgs1 arg1, TArgs2 args2);
    }
}