using Handsey.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey.Tests.Integration.ViewModels
{
    public class EmployeeViewModel : IVerifiable
    {
        protected List<IHandler> _handlerLog;

        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public EmployeeViewModel()
        {
            _handlerLog = new List<IHandler>();
        }

        public IHandler[] HandlerLog()
        {
            return _handlerLog.ToArray();
        }

        public void UpdateLog(IHandler handler)
        {
            if (_handlerLog == null)
                _handlerLog = new List<IHandler>();

            _handlerLog.Add(handler);
        }
    }
}