
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
            else
            {
                throw new AssertionFailedException(string.Format(failureMessage, args), e);
            }
        }

        public static void Fail(string failureMessage, params object[] args)
        {
            if (args == null || args.Length == 0)
            {
                throw new AssertionFailedException(failureMessage);
            }
            else
            {
                throw new AssertionFailedException(string.Format(failureMessage, args));
            }
        }
    }
}
