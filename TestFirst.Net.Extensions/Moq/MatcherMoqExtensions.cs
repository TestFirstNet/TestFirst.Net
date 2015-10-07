using System;
using System.Linq.Expressions;
using Moq;

namespace TestFirst.Net.Extensions.Moq
{
    /// <summary>
    /// Extensions to allow matchers to be used to verify arguments to mocks. To use 
    /// replace 'It.Is' with myMatcher.Verify() or myMatcher.Verify(consoleLoggingDiagnostics)
    /// <para>
    /// As an example
    /// </para>
    /// <para>
    ///     myThing.Setup(t => t.CallMyMethod(MyMatcher.With()...Verify())
    /// </para>
    /// <para>
    /// or if you use the <see cref="FluentMock{T}"/> you could write it as
    /// </para>
    /// <para>
    ///     myThing = AMock&lt;MyThing&gt;()
    ///         .WhereMethod(t => t.CallMyMethod(MyMatcher.With()...Verify())
    ///         .Returns(null)
    ///         .Instance
    /// </para>
    /// <para>
    /// If you want to log match diagnostics you can either use the console logger or your own one. For example
    /// </para>
    /// <para>
    ///     var diag = new ConsoleLoggingMatchDiagnostics();
    ///     ....
    ///     MyMatcher.With()...Verify(diag)
    /// </para>
    /// <para>
    ///  or the shorter
    /// </para>
    /// <para>
    ///     MyMatcher.With()...VerifyLogToConsole()
    /// </para>
    /// <para>
    ///  as a fuller example
    /// </para>
    /// <para>
    ///     myThing = AMock&lt;MyThing&gt;()
    ///         .WhereMethod(t => t.CallMyMethod(MyMatcher.With()...VerifyLogToConsole())
    ///         .Returns(null)
    ///         .Instance
    ///  </para>
    /// </summary>
    public static class MatcherMoqExtensions
    {
        /// <summary>
        /// Return a Moq version of this matcher.
        /// See <see cref="MatcherMoqExtensions"/> for usage
        /// </summary>
        /// <param name="matcher">The matcher to verify</param>
        /// <typeparam name="T">The type to match against</typeparam>
        /// <returns>The instance</returns>
        public static T Verify<T>(this IMatcher<T> matcher)
        {
            return It.Is(matcher.ToMoqExpression());
        }

        /// <summary>
        /// Return a Moq version of this matcher.
        /// See <see cref="MatcherMoqExtensions"/> for usage
        /// </summary>
        /// <param name="matcher">The matcher to verify</param>
        /// <typeparam name="T">The nullable type to match against</typeparam>
        /// <returns>The instance</returns>
        public static T Verify<T>(this IMatcher<T?> matcher) where T : struct 
        {
            return It.Is(matcher.ToMoqExpression());
        }

        /// <summary>
        /// Return a Moq version of this matcher which also logs the matching diagnostics to the console.
        /// See <see cref="MatcherMoqExtensions"/> for usage
        /// </summary>
        /// <param name="matcher">The matcher to verify &amp; log diagnostics to console</param>
        /// <typeparam name="T">The type to match against</typeparam>
        /// <returns>The instance</returns>
        public static T VerifyLogToConsole<T>(this IMatcher<T> matcher)
        {
            var diag = new ConsoleLoggingMatchDiagnostics("[MockVerifyLog] [" + matcher.GetType().Name + "] ");
            return matcher.VerifyLogTo(diag);
        }

        /// <summary>
        /// Return a Moq version of this matcher which also logs the matching diagnostics to the console.
        /// See <see cref="MatcherMoqExtensions"/> for usage
        /// </summary>
        /// <param name="matcher">The matcher to verify &amp; log diagnostics to console</param>
        /// <typeparam name="T">The type to match against</typeparam>
        /// <returns>The instance</returns>
        public static T VerifyLogToConsole<T>(this IMatcher<T?> matcher) where T : struct
        {
            var diag = new ConsoleLoggingMatchDiagnostics("[MockVerifyLog] [" + matcher.GetType().Name + "] ");
            return matcher.VerifyLogTo(diag);
        }

        /// <summary>
        /// Return a Moq version of this matcher which also logs to the given match diagnostics.
        /// See <see cref="MatcherMoqExtensions"/> for usage
        /// </summary>
        /// <param name="matcher">The matcher to verify and log to the given match diagnostics</param>
        /// <param name="diagnostics">The match diagnostics to log to</param>
        /// <typeparam name="T">The type to match against</typeparam>
        /// <returns>The instance</returns>
        public static T VerifyLogTo<T>(this IMatcher<T> matcher, IMatchDiagnostics diagnostics)
        {
            return It.Is(matcher.ToMoqExpression(diagnostics));
        }

        /// <summary>
        /// Return a Moq version of this matcher which also logs to the given match diagnostics.
        /// See <see cref="MatcherMoqExtensions"/> for usage
        /// </summary>
        /// <param name="matcher">The matcher to verify and log to the given match diagnostics</param>
        /// <param name="diagnostics">The match diagnostics to log to</param>
        /// <typeparam name="T">The nullable type to match against</typeparam>
        /// <returns>The instance</returns>
        public static T VerifyLogTo<T>(this IMatcher<T?> matcher, IMatchDiagnostics diagnostics) 
            where T : struct 
        {
            return It.Is(matcher.ToMoqExpression(diagnostics));
        }

        // Convert the matcher to an expression for Moq. As in It.Is(MyMatcher.ToMoq())
        private static Expression<Func<T, bool>> ToMoqExpression<T>(this IMatcher<T> matcher)
        {
            return (T actual) => matcher.Matches(actual);          
        }

        private static Expression<Func<T, bool>> ToMoqExpression<T>(this IMatcher<T> matcher, IMatchDiagnostics diagnostics)
        {
            return (T actual) => TryMatch(actual, matcher, diagnostics);          
        }

        // Convert the matcher to an expression for Moq. As in It.Is(MyMatcher.ToMoq()). For primitives
        private static Expression<Func<T, bool>> ToMoqExpression<T>(this IMatcher<T?> matcher) 
            where T : struct 
        {
            return (T actual) => matcher.Matches(actual);
        }

        private static Expression<Func<T, bool>> ToMoqExpression<T>(this IMatcher<T?> matcher, IMatchDiagnostics diagnostics) 
            where T : struct 
        {
            return (T actual) => TryMatch(actual, matcher, diagnostics);
        }

        private static bool TryMatch<T>(T actual, IMatcher<T> matcher, IMatchDiagnostics diagnostics)
        {
            try 
            {
                // force newline as diagnostics don't end with one and it messes up the console when interleaved with 
                // log4net to console logging
                return diagnostics.TryMatch(actual, matcher);
            }
            finally 
            {
                diagnostics.Text(string.Empty);
            }
        }

        private static bool TryMatch<T>(T actual, IMatcher<T?> matcher, IMatchDiagnostics diagnostics) 
            where T : struct 
        {
            try 
            {
                // force newline as diagnostics don't end with one and it messes up the console when interleaved with 
                // log4net to console logging
                return diagnostics.TryMatch(actual, matcher);
            }
            finally 
            {
                diagnostics.Text(string.Empty);
            }
        }
    }
}