using Handsey.Tests.Integration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey.Tests.Integration.Handlers
{
    public class SaveStateChangeHandler<TVersionable> : IChangeHandler<TVersionable>
        where TVersionable : IVersionable
    {
        public void Handle(IVersionable arg1)
        {
            throw new NotImplementedException("save changes to dummy thing");
        }
    }
}