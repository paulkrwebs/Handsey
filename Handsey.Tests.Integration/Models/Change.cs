using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey.Tests.Integration.Models
{
    public class Change
    {
        public string Property { get; private set; }

        public string Before { get; private set; }

        public string After { get; private set; }

        public Change(string property, string before, string after)
        {
            Property = property;
            Before = before;
            After = after;
        }
    }
}