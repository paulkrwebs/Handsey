using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey.Tests.Integration.Models
{
    /// <summary>
    /// </summary>
    /// <remarks>need to think this through</remarks>
    public class Employee : IVersionable
    {
        protected List<Change> ChangeLog { get; set; }

        public string FirstName { get; private set; }

        public virtual void Change(string firstname)
        {
            // fire changed
        }

        public Change[] Changes()
        {
            throw new NotImplementedException();
        }

        public void ClearHistory()
        {
            throw new NotImplementedException();
        }
    }
}