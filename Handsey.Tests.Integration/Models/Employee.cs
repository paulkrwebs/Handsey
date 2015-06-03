using Handsey.Tests.Integration.Handlers;
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
        private readonly IApplicaton _application;

        protected IApplicaton Application { get { return _application; } }

        protected List<Change> ChangeLog { get; private set; }

        public Guid Id { get; private set; }

        public string FirstName { get; private set; }

        public string LastName { get; private set; }

        public Employee(string firstName
            , string lastName)
            : this(ApplicationLocator.Instance
                    , firstName
                    , lastName)
        { }

        public Employee(IApplicaton application
            , string firstName
            , string lastName)
        {
            _application = application;

            FirstName = firstName;
            LastName = lastName;

            Id = Guid.NewGuid();
            ChangeLog = new List<Change>();
        }

        public virtual void Change(string firstname, string lastName)
        {
            LogChange("FirstName", FirstName, firstname);
            FirstName = firstname;

            LogChange("LastName", LastName, lastName);
            FirstName = firstname;

            FireChange();
        }

        protected virtual void FireChange()
        {
            // Fire change!!!
            Application.Invoke<IChangeHandler<Employee>>(h => h.Handle(this));
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

        public object IHandlesChange { get; set; }
    }
}