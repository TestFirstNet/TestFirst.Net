using NUnit.Framework;
using TestFirst.Net.Matcher;

namespace TestFirst.Net.Test.Matcher
{
    [TestFixture]
    public class ADecimalTest : BaseMatcherTest
    {
        [Test]
        public void EqualToTest()
        {
            AssertFails(4.999999999999999M, ADecimal.EqualTo(5)); 
            AssertPasses(5, ADecimal.EqualTo(5));
            AssertFails(5.000000000000001M, ADecimal.EqualTo(5));
        }

        [Test]
        public void GreaterThanTest()
        {
            AssertFails(10M, ADecimal.GreaterThan(10M));
            AssertFails(10M, ADecimal.GreaterThan(10));

            AssertPasses(10.000000000000000000000000001M, ADecimal.GreaterThan(10M));
        }

        [Test]
        public void GreaterOrEqualToTest()
        {
            AssertFails(0.999999999999999M, ADecimal.GreaterOrEqualTo(1));
            AssertPasses(1M, ADecimal.GreaterOrEqualTo(1));
            AssertPasses(1.000000000000000000000000001M, ADecimal.GreaterOrEqualTo(1));
        }

        [Test]
        public void LessThanTest()
        {
            AssertPasses(9M, ADecimal.LessThan(10));
            AssertPasses(9.999999999999999M, ADecimal.LessThan(10));
            AssertFails(10M, ADecimal.LessThan(10));
        }

        [Test]
        public void LessThanOrEqualToTest()
        {
            AssertPasses(9M, ADecimal.LessThanOrEqualTo(10));
            AssertPasses(10M, ADecimal.LessThanOrEqualTo(10));
            AssertFails(10.000000000000000000000000001M, ADecimal.LessThanOrEqualTo(10));
        }

        [Test]
        public void BetweenTest()
        {
            AssertPasses(10.000000000000000000000000001M, ADecimal.Between(10, 13));
            AssertPasses(11M, ADecimal.Between(10, 13));
            AssertPasses(12.999999999999999M, ADecimal.Between(10, 13));
            AssertFails(13M, ADecimal.Between(10, 13));
        }

        [Test]
        public void BetweenIncludingTest()
        {
            AssertFails(9.999999999999999M, ADecimal.BetweenIncluding(10, 13));
            AssertPasses(10M, ADecimal.BetweenIncluding(10, 13));
            AssertPasses(11M, ADecimal.BetweenIncluding(10, 13));
            AssertPasses(12M, ADecimal.BetweenIncluding(10, 13));
            AssertPasses(13M, ADecimal.BetweenIncluding(10, 13));
            AssertFails(13.000000000000000000000000001M, ADecimal.BetweenIncluding(10, 13));
        }

        [Test]
        public void NotTest()
        {
            AssertPasses(9.9999999999999999999999999990M, ADecimal.Not(10M));
            AssertFails(10M, ADecimal.Not(10));
            AssertPasses(10.000000000000000000000000001M, ADecimal.Not(10M));
        }

        [Test]
        public void NullTest()
        {
            AssertPasses(null, ADecimal.Null());
            AssertFails(10M, ADecimal.Null());
        }

        [Test]
        public void NotNullTest()
        {
            AssertPasses(10M, ADecimal.NotNull());
            AssertFails(null, ADecimal.NotNull());
        }
    }
}
