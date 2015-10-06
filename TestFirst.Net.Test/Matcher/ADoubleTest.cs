using NUnit.Framework;
using TestFirst.Net.Matcher;

namespace TestFirst.Net.Test.Matcher
{
    [TestFixture]
    public class ADoubleTest : BaseMatcherTest
    {
        [Test]
        public void EqualToTest()
        {
            AssertFails(4.999999999999999D, ADouble.EqualTo(5)); 
            AssertPasses(5D, ADouble.EqualTo(5));
            AssertFails(5.000000000000001D, ADouble.EqualTo(5));
        }

        [Test]
        public void EqualToTestMin()
        {
            AssertPasses(double.MinValue, ADouble.EqualTo(double.MinValue));
            AssertFails(double.MinValue + 0.000000000000001D, ADouble.EqualTo(double.MaxValue));
        }

        [Test]
        public void EqualToNull()
        {
            AssertPasses(null, ADouble.EqualTo(null));
            AssertFails(null, ADouble.EqualTo(1D));
            AssertFails(1D, ADouble.EqualTo(null));
        }

        [Test]
        public void EqualToTestMax()
        {
            AssertFails(1.797689999999e+308, ADouble.EqualTo(double.MaxValue));
            AssertPasses(double.MaxValue, ADouble.EqualTo(double.MaxValue));
        }

        [Test]
        public void GreaterThanTest()
        {
            AssertFails(10D, ADouble.GreaterThan(10));
            AssertPasses(10.000000000000001D, ADouble.GreaterThan(10));
        }

        [Test]
        public void GreaterOrEqualToTest()
        {
            AssertFails(0.999999999999999D, ADouble.GreaterOrEqualTo(1));
            AssertPasses(1D, ADouble.GreaterOrEqualTo(1));
            AssertPasses(1.000000000000001D, ADouble.GreaterOrEqualTo(1));
        }

        [Test]
        public void LessThanTest()
        {
            AssertPasses(9D, ADouble.LessThan(10));
            AssertPasses(9.999999999999999D, ADouble.LessThan(10));
            AssertFails(10D, ADouble.LessThan(10));
        }

        [Test]
        public void LessThanOrEqualToTest()
        {
            AssertPasses(9D, ADouble.LessThanOrEqualTo(10));
            AssertPasses(10D, ADouble.LessThanOrEqualTo(10));
            AssertFails(10.000000000000001D, ADouble.LessThanOrEqualTo(10));
        }

        [Test]
        public void BetweenTest()
        {
            AssertPasses(10.000000000000001D, ADouble.Between(10, 13));
            AssertPasses(11D, ADouble.Between(10, 13));
            AssertPasses(12.999999999999999D, ADouble.Between(10, 13));
            AssertFails(13D, ADouble.Between(10, 13));
        }

        [Test]
        public void BetweenIncludingTest()
        {
            AssertFails(9.999999999999999D, ADouble.BetweenIncluding(10, 13));
            AssertPasses(10D, ADouble.BetweenIncluding(10, 13));
            AssertPasses(11D, ADouble.BetweenIncluding(10, 13));
            AssertPasses(12D, ADouble.BetweenIncluding(10, 13));
            AssertPasses(13D, ADouble.BetweenIncluding(10, 13));
            AssertFails(13.000000000000001D, ADouble.BetweenIncluding(10, 13));
        }

        [Test]
        public void NotTest()
        {
            AssertPasses(9.999999999999999D, ADouble.Not(10));
            AssertFails(10D, ADouble.Not(10));
            AssertPasses(10.000000000000001D, ADouble.Not(10));
        }

        [Test]
        public void NullTest()
        {
            AssertPasses(null, ADouble.Null());
            AssertFails(10D, ADouble.Null());
        }

        [Test]
        public void NotNullTest()
        {
            AssertPasses(10D, ADouble.NotNull());
            AssertFails(null, ADouble.NotNull());
        }
    }
}
