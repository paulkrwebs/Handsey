using Handsey.Handlers;
using Handsey.Tests.TestObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey.Tests.TestObjects.Handlers
{
    public class VersionableHandler<TType> : IHandler<IVersionable>
        where TType : class, IVersionable, new()
    {
        public void Handle(IVersionable arg1)
        {
            throw new NotImplementedException();
        }
    }
}