using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey.Tests.Integration.Models
{
    public class Manager : Employee
    {
        public int TeamSize { get; private set; }

        public void Change(string firstname, int teamSize)
        {
            TeamSize = teamSize;
            // log change

            base.Change(firstname);
        }
    }
}