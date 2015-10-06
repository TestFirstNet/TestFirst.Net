using NUnit.Framework;
using TestFirst.Net.Matcher;

namespace TestFirst.Net.Test.Matcher
{
    [TestFixture]
    public class ACharTest : BaseMatcherTest
    {
        [Test]
        public void EqualToTest()
        {         
            AssertPasses('x', AChar.EqualTo('x'));
            AssertPasses(null, AChar.EqualTo(null));

            AssertFails('x', AChar.EqualTo('y'));
            AssertFails(null, AChar.NotEqualTo(null));
        }

        [Test]
        public void NotEqualToTest()
        {
            AssertPasses('x', AChar.NotEqualTo('y'));
            AssertPasses(null, AChar.NotEqualTo('y'));
            AssertPasses('x', AChar.NotEqualTo(null));

            AssertFails(null, AChar.NotEqualTo(null));
            AssertFails('x', AChar.NotEqualTo('x'));
        }

        [Test]
        public void NullTest()
        {
            AssertPasses(null, AChar.Null());

            AssertFails('x', AChar.Null());
        }

        [Test]
        public void NotNullTest()
        {
            AssertPasses('x', AChar.NotNull());
            AssertFails(null, AChar.NotNull());
        }
    }
}
