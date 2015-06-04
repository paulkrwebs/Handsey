using Handsey.Tests.Integration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey.Tests.Integration.Handlers
{
    public class AlertStateChangeHandler<TVersionable> : IChangeHandler<TVersionable>
        where TVersionable : IVersionable, IVerifiable
    {
        public AlertStateChangeHandler()
        {
        }

        public void Handle(TVersionable arg1)
        {
            // Send an alert to inform the administrator a versionable object has changed
            arg1.UpdateLog(this);
        }
    }
}