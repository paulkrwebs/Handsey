using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey.Tests.Integration.Models
{
    public class SupportTicket : IVersionable
    {
        protected List<Change> ChangeLog { get; set; }

        public Project Project { get; private set; }

        public int Id { get; private set; }

        public string Name { get; private set; }

        public SupportTicketPriority Priority { get; private set; }

        public SupportTicketStatus Status { get; private set; }

        public SupportTicketResolution Resolution { get; private set; }

        public string Description { get; private set; }

        public DateTime Created { get; private set; }

        public DateTime Updated { get; private set; }

        public Employee Assignee { get; private set; }

        public Employee Reporter { get; private set; }

        public Change[] Changes()
        {
            return ChangeLog.ToArray();
        }

        public void ClearHistory()
        {
            ChangeLog.Clear();
        }

        protected void LogChange(string propertyName, string before, string after)
        {
            ChangeLog.Add(new Change(propertyName, before, after));
        }
    }
}