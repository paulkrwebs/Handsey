using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey.Tests.Integration.Models
{
    public interface IVersionable
    {
        Change[] Changes();

        void ClearHistory();
    }
}