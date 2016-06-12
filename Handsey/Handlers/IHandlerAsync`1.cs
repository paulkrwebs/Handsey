using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey.Handlers
{
    public interface IHandlerAsync<TArgs1> : IHandler
    {
        Task HandleAsync(TArgs1 arg1);
    }
}