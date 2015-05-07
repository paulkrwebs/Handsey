﻿using Handsey.Tests.Integration.Handlers;
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

        protected List<Change> ChangeLog { get; set; }

        public string FirstName { get; private set; }

        public string LastName { get; private set; }

        public Employee()
            : this(ApplicationLocator.Instance)
        { }

        public Employee(IApplicaton application)
        {
            _application = application;
        }

        public virtual void Change(string firstname, string lastName)
        {
            LogChange("FirstName", FirstName, firstname);
            FirstName = firstname;

            LogChange("LastName", LastName, lastName);
            FirstName = firstname;

            // Fire change!!!
            Application.Invoke<IChangeHandler<IVersionable>>(h => h.Handle(this));
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