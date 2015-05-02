using Handsey.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey.Tests.Integration.Handlers
{
    public interface IModelMapperHandler<TFrom, TTo> : IHandler<TFrom, TTo>
    {
    }
}