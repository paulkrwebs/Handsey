using Handsey.Tests.Integration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey.Tests.Integration.Handlers
{
    public class SaveStateChangeHandler<TVersionable> : IChangeHandler<TVersionable>
        where TVersionable : IVersionable, IVerifiable
    {
        public SaveStateChangeHandler()
        {
        }

        public void Handle(TVersionable arg1)
        {
            // saves the changed made to a persisted storage area
            arg1.UpdateLog(this);
        }
    }
}