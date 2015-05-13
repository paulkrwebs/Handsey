using Handsey.Tests.Integration.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey.Tests.Integration.Handlers
{
    public class UpdateRequestHandler<TFromViewModel, TToViewModel>
        : IOneToOneHandler<UpdateEmployeeRequest<TFromViewModel>, UpdateEmployeeRequest<TToViewModel>>
        where TFromViewModel : EmployeeViewModel
        where TToViewModel : EmployeeViewModel
    {
        public void Handle(UpdateEmployeeRequest<TFromViewModel> arg1, UpdateEmployeeRequest<TToViewModel> args2)
        {
            //ApplicationLocator
            //    .Instance
            //    .Invoke<IOneToOneHandler<UpdateEmployeeRequest<DeveloperViewModel>, UpdateEmployeeResponse<DeveloperViewModel>>>(
            //    h => h.Handle(new UpdateEmployeeRequest<DeveloperViewModel>()
            //        , new UpdateEmployeeResponse<DeveloperViewModel>()
            //        ));

            throw new NotImplementedException();
        }
    }
}