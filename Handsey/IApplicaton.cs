using Handsey.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey
{
    public interface IApplicaton
    {
        IApplicationConfiguration ApplicationConfiguration { get; set; }

        ApplicationHandlers ApplicationHandlers { get; }

        void Initialise();

        void Invoke<THandle>(Action<THandle> trigger)
            where THandle : IHandles;
    }
}