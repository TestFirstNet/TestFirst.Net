using NUnit.Framework;
using TestFirst.Net.Matcher;

namespace TestFirst.Net.Test.Matcher
{
    [TestFixture]
    public class ALongTest : BaseMatcherTest
    {
        [Test]
        public void EqualToTest()
        {
            AssertPasses(null, ALong.EqualTo(null));
            AssertFails(0, ALong.EqualTo(null));
            AssertFails(null, ALong.EqualTo(0));

            AssertFails(4L, ALong.EqualTo(5L));
            AssertPasses(5L, ALong.EqualTo(5L));
            AssertFails(6L, ALong.EqualTo(5L));
        }

        [Test]
        public void GreaterThanTest()
        {
            AssertFails(9L, ALong.GreaterThan(10L));
            AssertFails(10L, ALong.GreaterThan(10L));
            AssertPasses(11L, ALong.GreaterThan(10L));
        }

        [Test]
        public void GreaterOrEqualToTest()
        {
            AssertFails(0L, ALong.GreaterOrEqualTo(1L));
            AssertPasses(1L, ALong.GreaterOrEqualTo(1L));
            AssertPasses(2L, ALong.GreaterOrEqualTo(1L));
        }

        [Test]
        public void LessThanTest()
        {
            AssertPasses(9L, ALong.LessThan(10L));
            AssertFails(10L, ALong.LessThan(10L));
            AssertFails(11L, ALong.LessThan(10L));
        }

        [Test]
        public void LessThanOrEqualToTest()
        {
            AssertPasses(9L, ALong.LessThanOrEqualTo(10L));
            AssertPasses(10L, ALong.LessThanOrEqualTo(10L));
            AssertFails(11L, ALong.LessThanOrEqualTo(10L));
        }

        [Test]
        public void BetweenTest()
        {
            AssertFails(10L, ALong.Between(10L, 13L));
            AssertPasses(11L, ALong.Between(10L, 13L));
            AssertPasses(12L, ALong.Between(10L, 13L));
            AssertFails(13L, ALong.Between(10L, 13L));
        }

        [Test]
        public void BetweenIncludingTest()
        {
            AssertFails(9L, ALong.BetweenIncluding(10L, 13L));
            AssertPasses(10L, ALong.BetweenIncluding(10L, 13L));
            AssertPasses(11L, ALong.BetweenIncluding(10L, 13L));
            AssertPasses(12L, ALong.BetweenIncluding(10L, 13L));
            AssertPasses(13L, ALong.BetweenIncluding(10L, 13L));
            AssertFails(14L, ALong.BetweenIncluding(10L, 13L));
        }

        [Test]
        public void NotTest()
        {
            AssertPasses(9L, ALong.Not(10L));
            AssertFails(10L, ALong.Not(10L));
            AssertPasses(11L, ALong.Not(10L));
        }

        [Test]
        public void NullTest()
        {
            AssertPasses(null, ALong.Null());
            AssertFails(10L, ALong.Null());
        }

        [Test]
        public void NotNullTest()
        {
            AssertPasses(10L, ALong.NotNull());
            AssertFails(null, ALong.NotNull());
        }
    }
}
