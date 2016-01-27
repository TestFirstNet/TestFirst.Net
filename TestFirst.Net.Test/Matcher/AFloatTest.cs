using NUnit.Framework;
using TestFirst.Net.Matcher;

namespace TestFirst.Net.Test.Matcher
{
    [TestFixture]
    public class AFloatTest : BaseMatcherTest
    {
        [Test]
        public void EqualToTest()
        {
            AssertFails(4.999999F, AFloat.EqualTo(5F));
            AssertPasses(5F, AFloat.EqualTo(5F));
            AssertFails(5.000001F, AFloat.EqualTo(5F));
        }

        [Test]
        public void GreaterThanTest()
        {
            AssertFails(10F, AFloat.GreaterThan(10.000001F));
            AssertPasses(10.000002F, AFloat.GreaterThan(10.000001F));
        }

        [Test]
        public void GreaterOrEqualToTest()
        {
            AssertFails(10F, AFloat.GreaterOrEqualTo(10.000001F));
            AssertPasses(10.000001F, AFloat.GreaterOrEqualTo(10.000001F));
            AssertPasses(10.000002F, AFloat.GreaterOrEqualTo(10.000001F));
        }

        [Test]
        public void LessThanTest()
        {
            AssertPasses(10.000001F, AFloat.LessThan(10.000002F));
            AssertFails(10.000002F, AFloat.LessThan(10.000002F));
            AssertFails(11F, AFloat.LessThan(10.000002F));
        }

        [Test]
        public void LessThanOrEqualToTest()
        {
            AssertPasses(10.000001F, AFloat.LessThanOrEqualTo(10.000002F));
            AssertPasses(10.000002F, AFloat.LessThanOrEqualTo(10.000002F));
            AssertFails(10.000003F, AFloat.LessThanOrEqualTo(10.000002F));
        }

        [Test]
        public void BetweenTest()
        {
            AssertFails(10F, AFloat.Between(10.000001F, 10.000003F));
            AssertFails(10.000001F, AFloat.Between(10.000001F, 10.000003F));
            AssertPasses(10.000002F, AFloat.Between(10.000001F, 10.000003F));
            AssertFails(10.000003F, AFloat.Between(10.000001F, 10.000003F));
            AssertFails(10.000004F, AFloat.Between(10.000001F, 10.000003F));
            AssertFails(11F, AFloat.Between(10.000001F, 10.000003F));
        }

        [Test]
        public void BetweenIncludingTest()
        {
            AssertFails(10F, AFloat.BetweenIncluding(10.000001F, 10.000003F));
            AssertPasses(10.000001F, AFloat.BetweenIncluding(10.000001F, 10.000003F));
            AssertPasses(10.000002F, AFloat.BetweenIncluding(10.000001F, 10.000003F)); 
            AssertPasses(10.000003F, AFloat.BetweenIncluding(10.000001F, 10.000003F));
            AssertFails(10.000004F, AFloat.BetweenIncluding(10.000001F, 10.000003F));
            AssertFails(11F, AFloat.BetweenIncluding(10.000001F, 10.000003F));
        }

        [Test]
        public void NotTest()
        {
            AssertPasses(10.000000F, AFloat.Not(10.000001F));
            AssertFails(10.000001F, AFloat.Not(10.000001F));
            AssertPasses(10.000002F, AFloat.Not(10.000001F));
        }

        [Test]
        public void NullTest()
        {
            AssertPasses(null, AFloat.Null());
            AssertFails(10F, AFloat.Null());
        }

        [Test]
        public void NotNullTest()
        {
            AssertPasses(10F, AFloat.NotNull());
            AssertFails(null, AFloat.NotNull());
        }
    }
}
