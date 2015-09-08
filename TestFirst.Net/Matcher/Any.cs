namespace TestFirst.Net.Matcher
{
    public static class Any
    {
        /// <summary>
        /// Returns a matcher which requires at least one of the provided matchers to
        /// match.
        /// </summary>
        /// <param name="matchers">the matchers to match. Not null</param>
        /// <typeparam name="T">The type of the object to match</typeparam>
        public static IMatcher<T> Of<T>(params IMatcher<T>[] matchers)
        {
            return Matchers.Any(matchers);
        }
    }
}

