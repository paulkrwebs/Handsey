using Handsey.Tests.Integration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Handsey.Tests.ProfilerHarness
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            int iterations = 0;

            if (!int.TryParse(args[0], out iterations))
                throw new ArgumentException("iterations parameter should be a integer");

            bool clearCache = false;

            if (!bool.TryParse(args[1], out clearCache))
                throw new ArgumentException("Clear Cache parameter should be a bool");

            ThreadingTest threadingTest = new ThreadingTest();

            threadingTest.Setup();
            threadingTest.TriggerHandlersIterationsTest(iterations);
        }
    }
}