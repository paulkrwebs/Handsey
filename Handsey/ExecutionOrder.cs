using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey
{
    public enum ExecutionOrder
    {
        /// <summary>
        /// Will be sorted after "InSequence" and before "Last"
        /// </summary>
        NotSet,

        /// <summary>
        /// Always first. If there are multiple Firsts then the order of First classes cannot be predicted
        /// </summary>
        First,

        /// <summary>
        /// Always last. If there are multiple Lasts then the order of Last classes cannot be predicted
        /// </summary>
        Last,

        /// <summary>
        /// Will be ordered above the NotSet order and depends on the type set by the user on the HandlerAfter attribute
        /// </summary>
        InSequence
    }
}