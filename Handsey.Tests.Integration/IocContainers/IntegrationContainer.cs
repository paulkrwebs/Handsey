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
        private readonly ConcurrentDictionary<Type, List<Func<object>>> _factories;
        private readonly IHandlerCallLog _handlerCallLog;

        public IntegrationContainer(IHandlerCallLog handlerCallLog)
        {
            _factories = new ConcurrentDictionary<Type, List<Func<object>>>();
            _handlerCallLog = handlerCallLog;
        }

        public void Register(Type from, Type to)
        {
            List<Func<object>> factory;

            _factories.AddOrUpdate(from, new List<Func<object>>() { MakeConstructor(to) }, (t, f) =>
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

            if (_factories.TryGetValue(typeof(TResolve), out factory))
                return factory.Select(f => (TResolve)f()).ToArray();

            return new TResolve[0];
        }

        private Func<object> MakeConstructor(Type to)
        {
            return () => to.GetConstructor(new[] { typeof(IHandlerCallLog) }).Invoke(new[] { _handlerCallLog });
        }

        public void ClearRegistrations()
        {
            _factories.Clear();
        }
    }
}