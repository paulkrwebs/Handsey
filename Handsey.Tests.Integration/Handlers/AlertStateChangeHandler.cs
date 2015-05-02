using Handsey.Tests.Integration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey.Tests.Integration.Handlers
{
    public class AlertStateChangeHandler<TVersionable> : IHandlesChange<TVersionable>
        where TVersionable : IVersionable
    {
        public void Handle(IVersionable arg1)
        {
            throw new NotImplementedException("Send email to dumy thing");
        }
    }
}