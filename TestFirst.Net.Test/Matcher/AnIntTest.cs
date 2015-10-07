using NUnit.Framework;
using TestFirst.Net.Matcher;

namespace TestFirst.Net.Test.Matcher
{
    [TestFixture]
    public class AnIntTest : BaseMatcherTest
    {
        [Test]
        public void EqualToTest()
        {
            AssertFails(4, AnInt.EqualTo(5));
            AssertPasses(5, AnInt.EqualTo(5));
            AssertFails(6, AnInt.EqualTo(5));
        }

        [Test]
        public void GreaterThanTest()
        {
            AssertFails(9, AnInt.GreaterThan(10));
            AssertFails(10, AnInt.GreaterThan(10));
            AssertPasses(11, AnInt.GreaterThan(10));
        }

        [Test]
        public void GreaterOrEqualToTest()
        {
            AssertFails(0, AnInt.GreaterOrEqualTo(1));
            AssertPasses(1, AnInt.GreaterOrEqualTo(1));
            AssertPasses(2, AnInt.GreaterOrEqualTo(1));
        }

        [Test]
        public void LessThanTest()
        {
            AssertPasses(9, AnInt.LessThan(10));
            AssertFails(10, AnInt.LessThan(10));
            AssertFails(11, AnInt.LessThan(10));
        }

        [Test]
        public void LessThanOrEqualToTest()
        {
            AssertPasses(9, AnInt.LessThanOrEqualTo(10));
            AssertPasses(10, AnInt.LessThanOrEqualTo(10));
            AssertFails(11, AnInt.LessThanOrEqualTo(10));
        }

        [Test]
        public void BetweenTest()
        {
            AssertFails(10, AnInt.Between(10, 13));
            AssertPasses(11, AnInt.Between(10, 13));
            AssertPasses(12, AnInt.Between(10, 13));
            AssertFails(13, AnInt.Between(10, 13));
        }

        [Test]
        public void BetweenIncludingTest()
        {
            AssertFails(9, AnInt.BetweenIncluding(10, 13));
            AssertPasses(10, AnInt.BetweenIncluding(10, 13));
            AssertPasses(11, AnInt.BetweenIncluding(10, 13));
            AssertPasses(12, AnInt.BetweenIncluding(10, 13));
            AssertPasses(13, AnInt.BetweenIncluding(10, 13));
            AssertFails(14, AnInt.BetweenIncluding(10, 13));
        }

        [Test]
        public void NotTest()
        {
            AssertPasses(9, AnInt.Not(10));
            AssertFails(10, AnInt.Not(10));
            AssertPasses(11, AnInt.Not(10));
        }

        [Test]
        public void NullTest()
        {
            AssertPasses(null, AnInt.Null());
            AssertFails(10, AnInt.Null());
        }

        [Test]
        public void NotNullTest()
        {
            AssertPasses(10, AnInt.NotNull());
            AssertFails(null, AnInt.NotNull());
        }
    }
}
