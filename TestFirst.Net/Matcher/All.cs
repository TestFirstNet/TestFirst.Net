using System;

namespace TestFirst.Net.Matcher
{
    public static class All
    {
        /// <summary>
        /// Returns a matcher which requires all of the provided matchers to
        /// match.
        /// </summary>
        /// <param name="matchers">the matchers to match. Not null</param>
        /// <typeparam name="T">The type of the object to match</typeparam>
        public static IMatcher<T> Of<T>(params IMatcher<T>[] matchers)
        {
            return Matchers.All<T>(matchers);
        }
    }
}

