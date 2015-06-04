using Handsey.Tests.Integration.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Handsey.Tests.Integration.Models;

namespace Handsey.Tests.Integration.Handlers
{
    public class ModelMapperHandler<TFrom, TTo>
        : IOneToOneHandler<TFrom, TTo>
        where TFrom : IVerifiable
        where TTo : IVerifiable
    {
        public void Handle(TFrom arg1, TTo arg2)
        {
            arg1.UpdateLog(this);
            arg2.UpdateLog(this);
        }
    }
}