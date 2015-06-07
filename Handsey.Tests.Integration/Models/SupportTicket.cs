using Handsey.Handlers;
using Handsey.Tests.Integration.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey.Tests.Integration.Models
{
    public class SupportTicket : IVersionable, IVerifiable
    {
        #region // Fields

        private readonly IApplicaton _application;

        protected List<Change> ChangeLog { get; private set; }

        protected List<IHandler> _handlerLog;

        public Project Project { get; private set; }

        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public SupportTicketPriority Priority { get; private set; }

        public SupportTicketStatus Status { get; private set; }

        public SupportTicketResolution Resolution { get; private set; }

        public string Description { get; private set; }

        public DateTime Created { get; private set; }

        public DateTime Updated { get; private set; }

        public Employee Assignee { get; private set; }

        public Employee Reporter { get; private set; }

        #endregion // Fields

        #region Constructors

        public SupportTicket(Project project
            , string name
            , SupportTicketPriority priority
            , SupportTicketStatus status
            , SupportTicketResolution resolution
            , string description
            , Employee assignee
            , Employee reporter)
            : this(ApplicationLocator.Instance
                    , project
                    , name
                    , priority
                    , status
                    , resolution
                    , description
                    , assignee
                    , reporter)
        { }

        public SupportTicket(
            IApplicaton application
            , Project project
            , string name
            , SupportTicketPriority priority
            , SupportTicketStatus status
            , SupportTicketResolution resolution
            , string description
            , Employee assignee
            , Employee reporter)
        {
            _application = application;

            Project = project;
            Name = name;
            Priority = priority;
            Status = status;
            Resolution = resolution;
            Description = description;
            Assignee = assignee;
            Reporter = reporter;

            Created = DateTime.Now;
            Id = Guid.NewGuid();
            ChangeLog = new List<Change>();
            _handlerLog = new List<IHandler>();
        }

        #endregion Constructors

        public void Open()
        {
            _application.Invoke<INewSupportTicketCreated>(h => h.Handle(this));
        }

        public void AssignTo(Employee assignee)
        {
            LogChange("Assignee", Assignee.Id.ToString(), assignee.Id.ToString());
            Assignee = assignee;

            Updated = DateTime.Now;

            // Fire change!!!
            _application.Invoke<IChangeHandler<SupportTicket>>(h => h.Handle(this));
        }

        public void ChangeDetails(string name
            , SupportTicketPriority priority
            , SupportTicketStatus status
            , string description)
        {
            LogChange("Name", Name, name);
            Name = name;

            LogChange("Priority", Priority.ToString(), priority.ToString());
            Priority = priority;

            LogChange("Status", Status.ToString(), status.ToString());
            Status = status;

            LogChange("Description", Description, description);
            Description = description;

            Updated = DateTime.Now;

            // Fire change!!!
            _application.Invoke<IChangeHandler<SupportTicket>>(h => h.Handle(this));
        }

        public void Resolve(SupportTicketResolution resolution)
        {
            LogChange("Resolution", Resolution.ToString(), resolution.ToString());
            Resolution = resolution;

            LogChange("Status", Status.ToString(), SupportTicketStatus.Closed.ToString());
            Status = SupportTicketStatus.Closed;

            Updated = DateTime.Now;
            _application.Invoke<IChangeHandler<SupportTicket>>(h => h.Handle(this));
        }

        public void ReOpen()
        {
            LogChange("Status", Status.ToString(), SupportTicketStatus.ReOpened.ToString());
            Status = SupportTicketStatus.ReOpened;

            Updated = DateTime.Now;

            // Fire change!!!
            _application.Invoke<IChangeHandler<SupportTicket>>(h => h.Handle(this));
        }

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

        public IHandler[] HandlerLog()
        {
            return _handlerLog.ToArray();
        }

        public void UpdateLog(IHandler handler)
        {
            _handlerLog.Add(handler);
        }
    }
}