using System.Collections.Generic;
using NUnit.Framework;
using TestFirst.Net.Util;

namespace TestFirst.Net.Test.Matcher
{
    /// <summary>
    /// Provides a number of convenience methods to test matchers
    /// </summary>
    public class BaseMatcherTest
    {
        protected void AssertPasses<T>(IEnumerable<T> items, IList<IMatcher<T>> matchers)
        {
            var itemsAsList = new List<T>(items);

            Assert.AreEqual(itemsAsList.Count, matchers.Count, "wrong number of matchers");

            for (int i = 0; i < itemsAsList.Count; i++)
            {
                var item = itemsAsList[i];
                var matcher = matchers[i];
                AssertPasses(item, matcher);
            }
        }

        protected void AssertFails<T>(IEnumerable<T> items, IList<IMatcher<T>> matchers)
        {
            var itemsAsList = new List<T>(items);

            Assert.AreEqual(itemsAsList.Count, matchers.Count, "wrong number of matchers");

            for (int i = 0; i < itemsAsList.Count; i++)
            {
                var item = itemsAsList[i];
                var matcher = matchers[i];
                AssertFails(item, matcher);
            }
        }

        /// <summary>
        /// Expect the matcher to pass on the given actual 
        /// </summary>
        protected void AssertPasses<T>(T actual, IMatcher<T> matcher)
        {
            var diagnostics = new MatchDiagnostics();
            if (!diagnostics.TryMatch(actual, matcher))
            {
                Assert.Fail("Matcher was expected to pass but didn't.\n" + diagnostics);
            }
          //  Console.WriteLine(diagnostics.ToString());
        }

        /// <summary>
        /// Expect the matcher to fail on the given actual 
        /// </summary>
        protected void AssertFails<T>(T actual, IMatcher<T> matcher)
        {
            var diagnostics = new MatchDiagnostics();
            if (diagnostics.TryMatch(actual, matcher))
            {
                Assert.Fail("Matcher was expected to fail but didn't for:\n" + actual.ToPrettyString() + "\nexpected:\n" + diagnostics);
            }
           // Console.WriteLine(diagnostics.ToString());
        }

        /// <summary>
        /// Convenience method to force a conversion from an array (or params) to an IEnumerable. This stops the 
        /// compiler complaining and allows for more fluent tests
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        protected static IEnumerable<T> Items<T>(params T[] items)
        {
            return new List<T>(items);
        }

        protected static T[] Array<T>(params T[] items)
        {
            return items;
        }  
    }
}
