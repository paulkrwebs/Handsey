using Handsey.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey
{
    public interface IHandlerSearch
    {
        IList<TypeInfo> Execute(TypeInfo toMatch, IList<TypeInfo> listToSearch);

        bool Compare(TypeInfo a, TypeInfo b);
    }
}