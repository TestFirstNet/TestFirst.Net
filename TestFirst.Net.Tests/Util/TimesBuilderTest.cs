using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NSubstitute;
using Shouldly;

namespace TestFirst.Net.Util
{
    [TestFixture]
    class TimesBuilderTest
    {
        [Test]
        public void test()
        {
            var buildFuncReturns = new object();
            var buildFunc = Substitute.For<Func<int, object>>();
            buildFunc.Invoke(Arg.Is<int>(3)).Returns<object>(buildFuncReturns);

            var actual = new TimesBuilder<int, object>(3,buildFunc).Times();

            actual.ShouldBeSameAs(buildFuncReturns);

            buildFunc.Received().Invoke(Arg.Is<int>(3));
        }
    }
}
