using System;
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
        /// <summary>
        /// Convenience method to force a conversion from an array (or params) to an IEnumerable. This stops the 
        /// compiler complaining and allows for more fluent tests
        /// </summary>
        /// <typeparam name="T">The type of item</typeparam>
        /// <param name="items">The array of items</param>
        /// <returns>The enumerable of items</returns>
        protected static IEnumerable<T> Items<T>(params T[] items)
        {
            return new List<T>(items);
        }

        protected static T[] Array<T>(params T[] items)
        {
            return items;
        }

        protected void AssertPasses(Action action, string errorMsg = null)
        {
            Exception thrown = null;
            try
            {
                action();
            }
            catch (Exception e)
            {
                thrown = e;
            }
            Assert.IsNull(thrown, "expect no failure" + AppendMsg(errorMsg));
        }

        protected void AssertFails(Action action, string errorMsg = null)
        {
            AssertionFailedException thrownAssertion = null;
            Exception thrown = null;
            try
            {
                action();
            }
            catch (AssertionFailedException e)
            {
                thrownAssertion = e;
            }
            catch (Exception e)
            {
                thrown = e;
            }

            Assert.IsNotNull(thrownAssertion, "expected assertion failure " + AppendMsg(errorMsg));
            Assert.IsNull(thrown, errorMsg, "expected assertion failure not this exception" + AppendMsg(errorMsg));
        }

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
        /// <param name="actual">The actual value</param>
        /// <param name="matcher">The matcher to match with</param>
        /// <typeparam name="T">The type of the value</typeparam>
        protected void AssertPasses<T>(T actual, IMatcher<T> matcher)
        {
            var diagnostics = new MatchDiagnostics();
            if (!diagnostics.TryMatch(actual, matcher))
            {
                Assert.Fail("Matcher was expected to pass but didn't.\n" + diagnostics);
            }
        }

        /// <summary>
        /// Expect the matcher to fail on the given actual 
        /// </summary>
        /// <param name="actual">The actual value</param>
        /// <param name="matcher">The matcher to match with</param>
        /// <typeparam name="T">The type of the value</typeparam>
        protected void AssertFails<T>(T actual, IMatcher<T> matcher)
        {
            var diagnostics = new MatchDiagnostics();
            if (diagnostics.TryMatch(actual, matcher))
            {
                Assert.Fail("Matcher was expected to fail but didn't for:\n" + actual.ToPrettyString() + "\nexpected:\n" + diagnostics);
            }
        }

        /// <summary>
        /// Expect the matcher to fail on the given actual. Tests the failure message
        /// </summary>
        /// <param name="actual">The actual value</param>
        /// <param name="matcher">The matcher to match with</param>
        /// <param name="messageMatcher">The matcher for the failure message</param>
        /// <typeparam name="T">The type of the value</typeparam>
        protected void AssertFails<T>(T actual, IMatcher<T> matcher, IMatcher<string> messageMatcher)
        {
            var diagnostics = new MatchDiagnostics();
            if (diagnostics.TryMatch(actual, matcher))
            {
                Assert.Fail("Matcher was expected to fail but didn't for:\n" + actual.ToPrettyString() + "\nexpected:\n" + diagnostics);
            } 
            else 
            {
                string msg = matcher.ToString();
                if (!messageMatcher.Matches(msg))
                {
                    Assert.Fail("Expected diagnostics failure message string match:\n" + messageMatcher + "\n but got '" + msg + "'");
                }
            }
        }

        private static string AppendMsg(string msg)
        {
            return msg == null ? string.Empty : ". " + msg;
        }
    }
}
