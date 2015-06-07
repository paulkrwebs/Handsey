using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey.Tests.Integration.Handlers
{
    public class NewSupportTicketCreated : INewSupportTicketCreated
    {
        public void Handle(Models.SupportTicket arg1)
        {
            arg1.UpdateLog(this);
        }
    }
}