using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey
{
    public class ApplicationConfigurationNotSetException : Exception
    {
        public ApplicationConfigurationNotSetException(string message)
            : base(message)
        { }
    }
}