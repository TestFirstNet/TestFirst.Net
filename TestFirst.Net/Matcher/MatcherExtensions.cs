using System;

namespace TestFirst.Net.Matcher
{
    public static class MatcherExtensions
    {
        /// <summary>
        /// Convenience method to assert an instance matches the given matcher
        /// </summary>
        /// <param name="matcher">the matcher to invoke</param>
        /// <param name="actual">the instance to pass to the matcher</param>        
        public static void AssertMatch<T>(this IMatcher<T> matcher, T actual)
        {
            AssertMatch(matcher, (Object) actual);
        }
        
        /// <summary>
        /// Convenience method to assert an instance matches the given matcher
        /// </summary>
        /// <param name="matcher">the matcher to invoke</param>
        /// <param name="actual">the instance to pass to the matcher</param>    
        public static void AssertMatch(this IMatcher matcher, Object actual)
        {
            var diag = new MatchDiagnostics();
            if (!diag.TryMatch(actual, matcher))
            {
                TestFirstAssert.Fail(Environment.NewLine + diag);
            }
        }

        /// <summary>
        /// Convenience method to return whether a matcher matched. Provides the match diagnostics to the matcher
        /// </summary>
        /// <param name="matcher">the matcher to invoke</param>
        /// <param name="actual">the instance to pass to the matcher</param>    
        /// <returns>true if matched</returns>
        public static bool Matches(this IMatcher matcher, Object actual)
        {
            return matcher.Matches(actual,NullMatchDiagnostics.Instance);            
        }
    }
}
