using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey.Utilities
{
    public class PerformCheck
    {
        private Func<bool> _check;

        protected PerformCheck(Func<bool> check)
        {
            _check = check;
        }

        /// <summary>
        /// Run the check
        /// </summary>
        /// <returns></returns>
        public bool Eval()
        {
            return this._check();
        }

        /// <summary>
        /// If the check is true then throw the exception created by the factory
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="factory"></param>
        public void Throw<TException>(Func<TException> factory)
            where TException : Exception, new()
        {
            if (this._check())
                throw factory();
        }

        public static PerformCheck IsNull(params Func<object>[] deferredEvalChain)
        {
            return new PerformCheck(() => deferredEvalChain.Any(e => e() == null));
        }

        public static PerformCheck IsNull(params object[] objs)
        {
            return new PerformCheck(() => objs.Any(o => o == null));
        }

        public static PerformCheck IsNull(object obj)
        {
            return new PerformCheck(() => obj == null);
        }

        public static PerformCheck IsTrue(Func<bool> check)
        {
            return new PerformCheck(() => check());
        }
    }
}