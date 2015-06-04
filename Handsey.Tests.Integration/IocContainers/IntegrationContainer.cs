using Handsey.Tests.Integration.Handlers;
using Handsey.Tests.Integration.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey.Tests.Integration.IocContainers
{
    public class IntegrationContainer : IIocContainer
    {
        private static object initLock = new object();

        /// <summary>
        ///
        /// </summary>
        /// <remarks>
        /// For testing integration only. This creates a new factory per thread, this means I can clear and
        /// check registrations without clearing the other threads registrations and other threads will still
        /// walk thorugh the whole handler search lifecycle. Allows us to check thread concurrency
        /// </remarks>
        [ThreadStatic]
        protected static ConcurrentDictionary<Type, List<Func<object>>> _factories;

        protected static ConcurrentDictionary<Type, List<Func<object>>> Factories
        {
            get
            {
                if (_factories == null)
                    lock (initLock)
                    {
                        if (_factories == null)
                            _factories = new ConcurrentDictionary<Type, List<Func<object>>>();
                    }

                return _factories;
            }
        }

        public void Register(Type from, Type to)
        {
            List<Func<object>> factory;

            Factories.AddOrUpdate(from, new List<Func<object>>() { MakeConstructor(to) }, (t, f) =>
                {
                    f.Add(MakeConstructor(to));
                    return f;
                });
        }

        public void Register(Type from, Type to, string name)
        {
            throw new NotImplementedException();
        }

        public TResolve[] ResolveAll<TResolve>()
        {
            List<Func<object>> factory;

            if (Factories.TryGetValue(typeof(TResolve), out factory))
                return factory.Select(f => (TResolve)f()).ToArray();

            return new TResolve[0];
        }

        private Func<object> MakeConstructor(Type to)
        {
            return () => to.GetConstructor(new Type[0]).Invoke(null);
        }

        public void ClearThreadRegistrations()
        {
            Factories.Clear();
        }
    }
}