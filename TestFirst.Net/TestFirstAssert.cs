
using System;

namespace TestFirst.Net
{
    public static class TestFirstAssert
    {
        public static void IsTrue(bool actual, string failMessage)
        {
            if (!actual)
            {
                throw new AssertionFailedException("Expected true but was false." + failMessage);
            }
        }

        public static void Fail(Exception e, string failureMessage, params object[] args)
        {
            if (args == null || args.Length == 0)
            {
                throw new AssertionFailedException(failureMessage, e);
            }
            throw new AssertionFailedException(string.Format(failureMessage, args), e);
        }

        public static void Fail(string failureMessage, params object[] args)
        {
            if (args == null || args.Length == 0)
            {
                throw new AssertionFailedException(failureMessage);
            }
            throw new AssertionFailedException(string.Format(failureMessage, args));
        }
    }
}
