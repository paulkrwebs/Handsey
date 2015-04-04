using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey.Utilities
{
    public static class NullCheck
    {
        public static bool IsNull(params Func<object>[] deferredEvalChain)
        {
            return deferredEvalChain.Any(e => e() == null);
        }

        public static bool IsNull(params object[] objs)
        {
            return objs.Any(o => o == null);
        }

        public static bool IsNull(object obj)
        {
            return obj == null;
        }

        public static void ThowIfNull<TException>(object obj, Func<TException> factory)
            where TException : Exception, new()
        {
            if (obj == null)
            {
                throw factory();
            }
        }
    }
}