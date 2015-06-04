using Handsey.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey.Tests.Integration
{
    public interface IVerifiable
    {
        IHandler[] HandlerLog();

        void UpdateLog(IHandler handler);
    }
}