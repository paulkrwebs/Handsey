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

        public static void ThrowIfNull<TException>(object obj, Func<TException> factory)
            where TException : Exception, new()
        {
            if (obj == null)
            {
                throw factory();
            }
        }

        public static void ThrowIfNull<TException>(Func<TException> factory, params object[] objs)
            where TException : Exception, new()
        {
            foreach (object obj in objs)
            {
                if (obj == null)
                {
                    throw factory();
                }
            }
        }
    }
}