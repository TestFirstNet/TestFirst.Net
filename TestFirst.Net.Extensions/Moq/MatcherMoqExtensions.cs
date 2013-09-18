using System;
using System.Linq.Expressions;
using Moq;
using TestFirst.Net.Matcher;

namespace TestFirst.Net.Extensions.Moq
{
    /// <summary>
    /// Extensions to allow matchers to be used to verify arguments to mocks. To use 
    /// replace 'It.Is' with myMatcher.Verify() or myMatcher.Verify(consoleLoggingDiagnostics)
    /// 
    /// As an example
    /// 
    ///     myThing.Setup(t=>t.CallMyMethod(MyMatcher.With()...Verify())
    /// 
    /// or if you use the <see cref="FluentMock{T}"/> you could write it as
    /// 
    ///     myThing = AMock&lt;MyThing&gt;()
    ///         .WhereMethod(t=>t.CallMyMethod(MyMatcher.With()...Verify())
    ///         .Returns(null)
    ///         .Instance
    /// 
    /// If you want to log match diagnostics you can either use the console logger or your own one. For example
    /// 
    ///     var diag = new ConsoleLoggingMatchDiagnostics();
    ///     ....
    ///     MyMatcher.With()...Verify(diag)
    /// 
    ///  or the shorter
    ///     
    ///     MyMatcher.With()...VerifyLogToConsole()
    /// 
    ///  as a fuller example
    /// 
    ///     myThing = AMock&lt;MyThing&gt;()
    ///         .WhereMethod(t=>t.CallMyMethod(MyMatcher.With()...VerifyLogToConsole())
    ///         .Returns(null)
    ///         .Instance
    ///  
    /// </summary>
    public static class MatcherMoqExtensions
    {
        /// <summary>
        /// Return a Moq version of this matcher.
        /// See <see cref="MatcherMoqExtensions"/> for usage
        /// </summary>
        public static T Verify<T>(this IMatcher<T> matcher)
        {
            return It.Is(matcher.ToMoqExpression());
        }

        /// <summary>
        /// Return a Moq version of this matcher.
        /// See <see cref="MatcherMoqExtensions"/> for usage
        /// </summary>
        public static T Verify<T>(this IMatcher<T?> matcher) where T:struct 
        {
            return It.Is(matcher.ToMoqExpression());
        }

        /// <summary>
        /// Return a Moq version of this matcher which also logs the matching diagnostics to the console.
        /// See <see cref="MatcherMoqExtensions"/> for usage
        /// </summary>
        public static T VerifyLogToConsole<T>(this IMatcher<T> matcher)
        {
            var diag = new ConsoleLoggingMatchDiagnostics("[MockVerifyLog] [" + matcher.GetType().Name + "] ");
            return matcher.VerifyLogTo(diag);
        }

        /// <summary>
        /// Return a Moq version of this matcher which also logs the matching diagnostics to the console.
        /// See <see cref="MatcherMoqExtensions"/> for usage
        /// </summary>
        public static T VerifyLogToConsole<T>(this IMatcher<T?> matcher) where T:struct
        {
            var diag = new ConsoleLoggingMatchDiagnostics("[MockVerifyLog] [" + matcher.GetType().Name + "] ");
            return matcher.VerifyLogTo(diag);
        }

        /// <summary>
        /// Return a Moq version of this matcher which also logs to the given match diagnostics.
        /// See <see cref="MatcherMoqExtensions"/> for usage
        /// </summary>
        public static T VerifyLogTo<T>(this IMatcher<T> matcher,IMatchDiagnostics diagnostics)
        {
            return It.Is(matcher.ToMoqExpression(diagnostics));
        }

        /// <summary>
        /// Return a Moq version of this matcher which also logs to the given match diagnostics.
        /// See <see cref="MatcherMoqExtensions"/> for usage
        /// </summary>
        public static T VerifyLogTo<T>(this IMatcher<T?> matcher,IMatchDiagnostics diagnostics) where T:struct 
        {
            return It.Is(matcher.ToMoqExpression(diagnostics));
        }

        /// Convert the matcher to an expression for Moq. As in It.Is(MyMatcher.ToMoq())
        private static Expression<Func<T,bool>> ToMoqExpression<T>(this IMatcher<T> matcher)
        {
            return (T actual) => matcher.Matches(actual);          
        }

        private static Expression<Func<T,bool>> ToMoqExpression<T>(this IMatcher<T> matcher,IMatchDiagnostics diagnostics)
        {
            return (T actual) => TryMatch(actual, matcher, diagnostics);          
        }

        /// Convert the matcher to an expression for Moq. As in It.Is(MyMatcher.ToMoq()). For primitives
        private static Expression<Func<T,bool>> ToMoqExpression<T>(this IMatcher<T?> matcher) where T:struct 
        {
            return (T actual) => matcher.Matches(actual);
        }

        private static Expression<Func<T,bool>> ToMoqExpression<T>(this IMatcher<T?> matcher,IMatchDiagnostics diagnostics) where T:struct 
        {
            return (T actual) => TryMatch(actual, matcher, diagnostics);
        }

        private static bool TryMatch<T>(T actual, IMatcher<T> matcher, IMatchDiagnostics diagnostics)
        {
            try 
            {
                //force newline as diagnostics don't end with one and it messes up the console when interleaved with 
                //log4net to console logging
                return diagnostics.TryMatch(actual, matcher);
            }
            finally 
            {
               diagnostics.Text("");
            }
        }

        private static bool TryMatch<T>(T actual, IMatcher<T?> matcher, IMatchDiagnostics diagnostics) where T:struct 
        {
            try 
            {
                //force newline as diagnostics don't end with one and it messes up the console when interleaved with 
                //log4net to console logging
                return diagnostics.TryMatch(actual, matcher);
            }
            finally 
            {
               diagnostics.Text("");
            }
        }
    }
}