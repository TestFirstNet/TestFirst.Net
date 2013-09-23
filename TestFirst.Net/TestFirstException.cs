using System;

namespace TestFirst.Net
{
    public class TestFirstException:Exception
    {
        public TestFirstException(string message) : base(message)
        {
        }

        public TestFirstException(string message, Exception innerException): base(message, innerException)
        {
        }

    }
}
