using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey.Tests
{
    [TestFixture]
    public class HandlersSortTests
    {
        [Test]
        public void Sort_HandlerInfoEnumberable_ListIsNullSoThrowsException()
        {
            HandlersSort handlersSort = new HandlersSort();

            Assert.That(() => handlersSort.Sort(null), Throws.Exception.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void Sort_HandlerInfoEnumberable_NotAllHandlersHaveATypeSoExceptionThrown()
        {
            List<HandlerInfo> toSort = new List<HandlerInfo>()
            {
                new HandlerInfo()
                {
                    ExecutionOrder = ExecutionOrder.InSequence,
                    Type = typeof(Int64),
                },
                new HandlerInfo()
                {
                    ExecutionOrder = ExecutionOrder.NotSet,
                }
            };

            HandlersSort handlersSort = new HandlersSort();
            Assert.That(() => handlersSort.Sort(toSort), Throws.Exception.TypeOf<ArgumentException>());
        }

        [Test]
        public void Sort_HandlerInfoEnumberable_ListIsSortedCorrectly()
        {
            List<HandlerInfo> toSort = new List<HandlerInfo>()
            {
                new HandlerInfo()
                {
                    ExecutionOrder = ExecutionOrder.InSequence,
                    Type = typeof(Int64),
                    ExecutesAfter = new[] { typeof(Int16), typeof(Int32) }
                },
                new HandlerInfo()
                {
                    ExecutionOrder = ExecutionOrder.NotSet,
                    Type = typeof(string)
                },
                new HandlerInfo()
                {
                    ExecutionOrder = ExecutionOrder.Last,
                    Type = typeof(bool)
                },
                new HandlerInfo()
                {
                    ExecutionOrder = ExecutionOrder.First,
                    Type = typeof(byte)
                },
                new HandlerInfo()
                {
                    ExecutionOrder = ExecutionOrder.InSequence,
                    Type = typeof(Int16)
                },
                new HandlerInfo()
                {
                    ExecutionOrder = ExecutionOrder.InSequence,
                    Type = typeof(Int32)
                },
            };

            HandlersSort handlersSort = new HandlersSort();
            IList<HandlerInfo> sorted = handlersSort.Sort(toSort);

            Assert.That(sorted[0].Type, Is.EqualTo(typeof(byte)));
            Assert.That(sorted[1].Type, Is.EqualTo(typeof(Int16)).Or.EqualTo(typeof(Int32)));
            Assert.That(sorted[2].Type, Is.EqualTo(typeof(Int16)).Or.EqualTo(typeof(Int32)));
            Assert.That(sorted[3].Type, Is.EqualTo(typeof(Int64)));
            Assert.That(sorted[4].Type, Is.EqualTo(typeof(string)));
            Assert.That(sorted[5].Type, Is.EqualTo(typeof(bool)));
        }
    }
}