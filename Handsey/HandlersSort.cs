using Handsey.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey
{
    public class HandlersSort : IHandlersSort
    {
        private delegate bool TryCompare(HandlerInfo a, HandlerInfo b, out int result);

        private static ConcurrentQueue<TryCompare> _compareStrategies;

        static HandlersSort()
        {
            _compareStrategies = new ConcurrentQueue<TryCompare>();
            _compareStrategies.Enqueue(AIsFirst);
            _compareStrategies.Enqueue(BIsFirst);
            _compareStrategies.Enqueue(AIsLast);
            _compareStrategies.Enqueue(BIsLast);
            _compareStrategies.Enqueue(AIsNotSet);
            _compareStrategies.Enqueue(BIsNotSet);
            _compareStrategies.Enqueue(AIsInSequenceCheckExecutesAfter);
            _compareStrategies.Enqueue(BIsInSequenceCheckExecutesAfter);
        }

        public IList<HandlerInfo> Sort(IEnumerable<HandlerInfo> toSort)
        {
            PerformCheck.IsNull(toSort)
                .Throw<ArgumentNullException>(() =>
                    new ArgumentNullException("List to sort cannot be null")
                    );

            // Making a copy makes it immutable
            List<HandlerInfo> toSortAsList = toSort.ToList();

            PerformCheck.IsTrue(() => toSortAsList.Any(h => h.Type == null))
                .Throw<ArgumentException>(() =>
                    new ArgumentException("One or more handler does not have a Type property set")
                    );

            toSortAsList.Sort(Compare);

            return toSortAsList;
        }

        private static bool AIsFirst(HandlerInfo a, HandlerInfo b, out int result)
        {
            if (a.ExecutionOrder == ExecutionOrder.First)
            {
                result = -1;
                return true;
            }

            result = 0;
            return false;
        }

        private static bool BIsFirst(HandlerInfo a, HandlerInfo b, out int result)
        {
            if (b.ExecutionOrder == ExecutionOrder.First)
            {
                result = 1;
                return true;
            }

            result = 0;
            return false;
        }

        private static bool AIsLast(HandlerInfo a, HandlerInfo b, out int result)
        {
            if (a.ExecutionOrder == ExecutionOrder.Last)
            {
                result = 1;
                return true;
            }

            result = 0;
            return false;
        }

        private static bool BIsLast(HandlerInfo a, HandlerInfo b, out int result)
        {
            if (b.ExecutionOrder == ExecutionOrder.Last)
            {
                result = -1;
                return true;
            }

            result = 0;
            return false;
        }

        private static bool AIsNotSet(HandlerInfo a, HandlerInfo b, out int result)
        {
            if (a.ExecutionOrder == ExecutionOrder.NotSet)
            {
                result = 1;
                return true;
            }

            result = 0;
            return false;
        }

        private static bool BIsNotSet(HandlerInfo a, HandlerInfo b, out int result)
        {
            if (b.ExecutionOrder == ExecutionOrder.NotSet)
            {
                result = -1;
                return true;
            }

            result = 0;
            return false;
        }

        private static bool AIsInSequenceCheckExecutesAfter(HandlerInfo a, HandlerInfo b, out int result)
        {
            if (a.ExecutionOrder == ExecutionOrder.InSequence)
            {
                result = CompareSequence(a, b);
                return true;
            }

            result = 0;
            return false;
        }

        private static bool BIsInSequenceCheckExecutesAfter(HandlerInfo a, HandlerInfo b, out int result)
        {
            if (b.ExecutionOrder == ExecutionOrder.InSequence)
            {
                result = CompareSequence(b, a);
                return true;
            }

            result = 0;
            return false;
        }

        public static int Compare(HandlerInfo a, HandlerInfo b)
        {
            int result = 0;

            foreach (TryCompare compareStrategy in _compareStrategies)
            {
                if (compareStrategy(a, b, out result))
                    return result;
            }

            return result;
        }

        private static int CompareSequence(HandlerInfo a, HandlerInfo b)
        {
            if (ExecutesAfter(b, a))
                return 1;

            if (ExecutesAfter(a, b))
                return -1;

            return 0;
        }

        private static bool ExecutesAfter(HandlerInfo executesFirst, HandlerInfo executesSecond)
        {
            if (executesSecond == null)
                return false;

            if (executesSecond.ExecutesAfter == null)
                return false;

            return executesSecond.ExecutesAfter.Any(t => t == executesFirst.Type);
        }
    }
}