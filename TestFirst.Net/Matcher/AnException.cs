using System;

namespace TestFirst.Net.Matcher
{
    public class AnException : PropertyMatcher<Exception>
    {
        // for refactor friendly support
        private static readonly Exception PropertyNames = null;

        public static IMatcher<Exception> Any()
        {
            return AnInstance.Any();
        }

        public static AnException Of()
        {
            return With();
        }

        public static AnException With()
        {
            return new AnException();
        }

        public AnException Message(string val)
        {
            Message(AString.EqualTo(val));
            return this;
        }

        public AnException Message(IMatcher<string> matcher)
        {
            WithProperty(() => PropertyNames.Message, matcher);
            return this;
        }

        /// <summary>
        /// Ensure exception is not a Moq exception or any other mocking exception used in TestFirst
        /// </summary>
        /// <returns>The exception matcher</returns>
        public AnException NotMockException()
        {
            var text1 = AString.Not(AString.ContainingOfAnyCase("mock behavior Strict"));
            var text2 = AString.Not(AString.ContainingOfAnyCase("mock must have a corresponding setup"));

            Message(text1);
            Message(text2);

            StackTrace(text1);
            StackTrace(text2);

            return this;
        }
        
        public AnException TypeMatching<T>(IMatcher<T> matcher) 
            where T : class
        {
            Type<T>();
            WithMatcher("Exception", e => e as T, matcher);
            return this;
        }

        public AnException Type<T>()
        {
            WithMatcher(AnInstance.OfType<T>());
            return this;
        }

        /// <summary>
        /// In cases where the underlying exception class is not visible (like in third party libs)
        /// </summary>
        /// <param name="fullTypeName">the expected fully qualified type name of the exception</param>
        /// <returns>The exception matcher</returns>
        public AnException Type(string fullTypeName)
        {
            Type(AString.EqualTo(fullTypeName));
            return this;
        }

        /// <summary>
        /// In cases where the underlying exception class is not visible (like in third party libs)
        /// </summary>
        /// <param name="typeMatcher">the matcher to use to match the type against</param>
        /// <returns>The exception matcher</returns>
        public AnException Type(IMatcher<string> typeMatcher)
        {
            WithMatcher("GetType().FullName", (Exception e) => e.GetType().FullName, typeMatcher);
            return this;
        }

        public AnException StackTrace(IMatcher<string> matcher)
        {
            WithProperty(() => PropertyNames.StackTrace, matcher);
            return this;
        }

        public AnException Source(IMatcher<string> matcher)
        {
            WithProperty(() => PropertyNames.Source, matcher);
            return this;
        }
    }
}
