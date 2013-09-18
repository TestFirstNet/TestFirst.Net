using System;

namespace TestFirst.Net
{
    /// <summary>
    /// Use our own Assertion failed exception so we don't force clients to be forced into using any particular 
    /// unit testing framework
    /// </summary>
    public class AssertionFailedException  : TestFirstException
    {
        public AssertionFailedException(string message) : base(message)
        {
        }

        public AssertionFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
