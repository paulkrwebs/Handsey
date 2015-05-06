using Handsey.Handlers;
using Handsey.Tests.Integration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey.Tests.Integration.Handlers
{
    public interface IChangeHandler<TVersionable> : IHandler<IVersionable>
        where TVersionable : IVersionable
    { }
}