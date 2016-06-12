using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey.Handlers
{
    public interface IHandlerAsync<TArgs1, TArgs2, TArgs3> : IHandler
    {
        Task Handle(TArgs1 arg1, TArgs2 args2, TArgs3 args3);
    }
}