using NUnit.Framework;
using TestFirst.Net.Matcher;

namespace TestFirst.Net.Test.Matcher
{
    [TestFixture]
    public class AShortTest : BaseMatcherTest
    {
        [Test]
        public void EqualToTest()
        {
            AssertPasses(null, AShort.EqualTo(null));
            AssertFails(0, AShort.EqualTo(null));
            AssertFails(null, AShort.EqualTo(0));
            
            AssertFails(4, AShort.EqualTo(5));
            AssertPasses(5, AShort.EqualTo(5));
            AssertFails(6, AShort.EqualTo(5));
        }

        [Test]
        public void GreaterThanTest()
        {
            AssertFails(9, AShort.GreaterThan(10));
            AssertFails(10, AShort.GreaterThan(10));
            AssertPasses(11, AShort.GreaterThan(10));
        }

        [Test]
        public void GreaterOrEqualToTest()
        {
            AssertFails(0, AShort.GreaterOrEqualTo(1));
            AssertPasses(1, AShort.GreaterOrEqualTo(1));
            AssertPasses(2, AShort.GreaterOrEqualTo(1));
        }

        [Test]
        public void LessThanTest()
        {
            AssertPasses(9, AShort.LessThan(10));
            AssertFails(10, AShort.LessThan(10));
            AssertFails(11, AShort.LessThan(10));
        }

        [Test]
        public void LessThanOrEqualToTest()
        {
            AssertPasses(9, AShort.LessThanOrEqualTo(10));
            AssertPasses(10, AShort.LessThanOrEqualTo(10));
            AssertFails(11, AShort.LessThanOrEqualTo(10));
        }

        [Test]
        public void BetweenTest()
        {
            AssertFails(10, AShort.Between(10, 13));
            AssertPasses(11, AShort.Between(10, 13));
            AssertPasses(12, AShort.Between(10, 13));
            AssertFails(13, AShort.Between(10, 13));
        }

        [Test]
        public void BetweenIncludingTest()
        {
            AssertFails(9, AShort.BetweenIncluding(10, 13));
            AssertPasses(10, AShort.BetweenIncluding(10, 13));
            AssertPasses(11, AShort.BetweenIncluding(10, 13));
            AssertPasses(12, AShort.BetweenIncluding(10, 13));
            AssertPasses(13, AShort.BetweenIncluding(10, 13));
            AssertFails(14, AShort.BetweenIncluding(10, 13));
        }

        [Test]
        public void NotTest()
        {
            AssertPasses(9, AShort.Not(10));
            AssertFails(10, AShort.Not(10));
            AssertPasses(11, AShort.Not(10));
        }

        [Test]
        public void NullTest()
        {
            AssertPasses(null, AShort.Null());
            AssertFails(10, AShort.Null());
        }

        [Test]
        public void NotNullTest()
        {
            AssertPasses(10, AShort.NotNull());
            AssertFails(null, AShort.NotNull());
        }

        protected void AssertPasses(short actual, IMatcher<short?> matcher)
        {
            base.AssertPasses(actual,matcher);
        }

        protected void AssertFails(short actual, IMatcher<short?> matcher)
        {
            base.AssertFails(actual, matcher);
        }
    }
}
