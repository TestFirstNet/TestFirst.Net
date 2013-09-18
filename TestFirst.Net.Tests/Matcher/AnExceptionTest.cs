using System;
using NUnit.Framework;
using TestFirst.Net.Matcher;

namespace TestFirst.Net.Test.Matcher
{
    [TestFixture]
    public class AnExceptionTest:BaseMatcherTest
    {
        [Test]
        public void MessageMatchTest()
        {
            AssertPasses(new MyException("MyMessage"), AnException.With().Message("MyMessage"));
            AssertPasses(new MyException("MyMessage"), AnException.With().Message(AString.StartingWith("My")));


            AssertFails(new MyException("MyMessage"), AnException.With().Message("OtherMessage"));
            AssertFails(new MyException("MyMessage"), AnException.With().Message(AString.StartingWith("Other")));
        }

        [Test]
        public void TypeMatchTest()
        {         
            AssertPasses(new MyException(), AnException.With().Type<Exception>());
            AssertPasses(new MyException(), AnException.With().Type<MyException>());
            AssertPasses(new MyException(), AnException.With().Type(typeof(MyException).FullName));

            AssertFails(new MyException(), AnException.With().Type<InvalidOperationException>());
            AssertFails(new MyException(), AnException.With().Type(typeof(Exception).FullName));
        }
        
        [Test]
        public void StackTraceMatchTest()
        {
            AssertPasses(NewThrownException(), AnException.With().StackTrace(AString.Containing("NewThrownException()")));
            AssertFails(NewThrownException(), AnException.With().StackTrace(AString.Containing("SomeOtherMethod()")));
        }

        private MyException NewThrownException()
        {
            try
            {
                throw new MyException();
            }
            catch (MyException e)
            {
                return e;
            }
        }
    }

    public class MyException : Exception {
        public MyException()
        {}

        public MyException(String msg):base(msg)
        {}
    }
}
