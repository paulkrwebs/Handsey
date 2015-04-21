using Handsey.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey.Tests.TestObjects.Handlers
{
    public interface IOneToOneDataPopulation<TFrom, TTo> : IHandler<TFrom, TTo>
    {
        void Handle(TFrom arg1, TTo args2);
    }
}
