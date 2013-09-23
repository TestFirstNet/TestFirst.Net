using System;
using NUnit.Framework;

namespace TestFirst.Net.Extensions.NUnit
{
    /// <summary>
    /// Marks a test as under development and is not expected to pass yet. Useful when writing tests first before the implementation exists.
    /// 
    /// If the test passes this will cause the test to fail
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class NotImplementedYet : ExpectedExceptionAttribute
    {
        public NotImplementedYet()
        {
            UserMessage = "Expected test to fail as this feature is marked as NotImplementedYet. However test passed so failing test";
        }
    }
}
