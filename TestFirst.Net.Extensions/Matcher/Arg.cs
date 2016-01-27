using TestFirst.Net.Extensions.Moq;
using TestFirst.Net.Matcher;

namespace TestFirst.Net.Extensions.Matcher
{
    /// <summary>
    /// Fluency helper to make Moq/Matcher integration nicer
    /// </summary>
    public static class Arg
    {
        private static bool _verifyLogToConsole;

        /// <summary>
        /// Ensure <see cref="Is{T}(IMatcher{T})"/> Moq verification also logs the matching to the console. Useful during debugging
        /// of tests
        /// </summary>
        /// <param name="b">Whether to verify to console</param>
        public static void UseVerifyToConsole(bool b)
        {
            _verifyLogToConsole = b;
        }

        public static T IsAny<T>()
        {
            if (_verifyLogToConsole)
            {
                return AnInstance.Any<T>().VerifyLogToConsole();
            }
            return AnInstance.Any<T>().Verify();
        }

        /// <summary>
        /// For Moq matcher verify. Instead of 
        /// <para>
        ///     MyMatcher.With()...Verify() 
        /// </para>
        /// <para>
        /// do 
        /// </para>
        /// <para>
        ///     Arg.Is(MyMatcher.With()...)
        /// </para>
        /// </summary>
        /// <param name="matcher">The matcher</param>
        /// <returns>The instance</returns>
        /// <typeparam name="T">The type of object to match against</typeparam>
        public static T Is<T>(IMatcher<T> matcher)
        {
            if (_verifyLogToConsole)
            {
                return matcher.VerifyLogToConsole();
            }
            return matcher.Verify();
        }

        public static T Is<T>(IMatcher<T?> matcher) where T : struct
        {
            if (_verifyLogToConsole)
            {
                return matcher.VerifyLogToConsole();
            }
            return matcher.Verify();
        }

        /// <summary>
        /// Use an instance matcher to allow passing in of an enum where a nullable enum is require
        /// </summary>
        /// <typeparam name="T">The type of instance to check</typeparam>
        /// <param name="val">The value to assert is not null</param>
        /// <returns>The value</returns>
        public static T? IsNotNull<T>(T val) where T : struct
        {
            if (_verifyLogToConsole)
            {
                return AnInstance.EqualTo(val).VerifyLogToConsole();
            }
            return AnInstance.EqualTo(val).Verify();
        }
    }
}
