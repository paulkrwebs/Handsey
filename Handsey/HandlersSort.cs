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
            PerformCheck.IsNull(toSort).Throw<ArgumentNullException>(() => new ArgumentNullException("List to sort cannot be null"));

            List<HandlerInfo> toSortAsList = toSort.ToList();

            PerformCheck.IsTrue(() => toSortAsList.Any(h => h.Type == null)).Throw<ArgumentException>(() => new ArgumentException("One or more handler does not have a Type property set"));

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

        //public static int Compare(HandlerInfo a, HandlerInfo b)
        //{
        //    if (a.ExecutionOrder == ExecutionOrder.First)
        //        return -1;

        //    if (b.ExecutionOrder == ExecutionOrder.First)
        //        return 1;

        //    if (a.ExecutionOrder == ExecutionOrder.Last)
        //        return 1;

        //    if (b.ExecutionOrder == ExecutionOrder.Last)
        //        return -1;

        //    if (a.ExecutionOrder == ExecutionOrder.NotSet)
        //        return 1;

        //    if (b.ExecutionOrder == ExecutionOrder.NotSet)
        //        return -1;

        //    if (a.ExecutionOrder == ExecutionOrder.InSequence)
        //        return CompareSequence(a, b);

        //    if (b.ExecutionOrder == ExecutionOrder.InSequence)
        //        return CompareSequence(b, a);

        //    return 0;
        //}

        private static int CompareSequence(HandlerInfo a, HandlerInfo b)
        {
            if (a.ExecutesAfter != null && a.ExecutesAfter.Any(t => t == b.Type))
                return 1;

            if (b.ExecutesAfter != null && b.ExecutesAfter.Any(t => t == a.Type))
                return -1;

            return 0;
        }
    }
}