using System;
using NSubstitute;
using NUnit.Framework;
using Shouldly;

namespace TestFirst.Net.Util
{
    [TestFixture]
    public class TimesBuilderTest
    {
        [Test]
        public void Test()
        {
            var buildFuncReturns = new object();
            var buildFunc = Substitute.For<Func<int, object>>();
            buildFunc.Invoke(Arg.Is<int>(3)).Returns<object>(buildFuncReturns);

            var actual = new TimesBuilder<int, object>(3, buildFunc).Times();

            actual.ShouldBeSameAs(buildFuncReturns);

            buildFunc.Received().Invoke(Arg.Is<int>(3));
        }
    }
}
