using Handsey.Tests.Integration.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey.Tests.Integration.Handlers
{
    public class UpdateRequestHandler<TFromViewModel, TToViewModel>
        : IOneToOneHandler<UpdateEmployeeRequest<TFromViewModel>, UpdateEmployeeResponse<TToViewModel>>
        where TFromViewModel : EmployeeViewModel, IVerifiable
        where TToViewModel : EmployeeViewModel, IVerifiable
    {
        public void Handle(UpdateEmployeeRequest<TFromViewModel> arg1, UpdateEmployeeResponse<TToViewModel> arg2)
        {
            arg1.Employee.UpdateLog(this);
            arg2.Employee.UpdateLog(this);
        }
    }
}