using System;
using NUnit.Framework;

namespace TestFirst.Net.Extensions.Moq
{
    [TestFixture]
    public class MockPropertyGetTest
    {
        [Test]
        public void InvokeCountOkPasses()
        {
            var mock = FluentMock<IMyTestInterface>.With()
                .WhereMethod(x => x.Foo).IsCalled(3).Times()
                .Returns("hi");
            var instance = mock.Instance;

            var foo = instance.Foo;
            foo = instance.Foo;
            foo = instance.Foo;

            mock.OnScenarioEnd();    
        }

        [Test]
        public void InvokeCountTooLowFails()
        {
            var mock = FluentMock<IMyTestInterface>.With()
                .WhereMethod(x => x.Foo).IsCalled(3).Times()
                .Returns("hi");
            var instance = mock.Instance;

            var foo = instance.Foo;
            foo = instance.Foo;

            Exception error = null;
            try
            {
                mock.OnScenarioEnd();
            }
            catch (Exception e)
            {
                error = e;
            }
            Assert.NotNull(error, "expected failure on too few invokes");
            Assert.IsTrue(error.Message.Contains("unexpectedly performed 2 times"));
            Assert.IsTrue(error.Message.Contains("expected 3 times"));

        }

        [Test]
        public void InvokeCountTooHighFails()
        {
            var mock = FluentMock<IMyTestInterface>.With()
                .WhereMethod(x => x.Foo).IsCalled(3).Times()
                .Returns("hi");
            var instance = mock.Instance;

            var foo = instance.Foo;
            foo = instance.Foo;
            foo = instance.Foo;

            Exception error = null;
            try
            {
                foo = instance.Foo;
            }
            catch (Exception e)
            {
                error = e;
            }
            Assert.NotNull(error, "expected failure on too many invokes");
            Assert.IsTrue(error.Message.Contains("unexpectedly performed 4 times"));
            Assert.IsTrue(error.Message.Contains("expected 3 times"));
        }

        public interface IMyTestInterface
        {
            string Foo { get; }
        }
    }
}
