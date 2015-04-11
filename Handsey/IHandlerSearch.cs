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
        IEnumerable<HandlerInfo> Execute(HandlerInfo toMatch, IList<HandlerInfo> listToSearch);

        bool Compare(HandlerInfo a, HandlerInfo b);
    }
}