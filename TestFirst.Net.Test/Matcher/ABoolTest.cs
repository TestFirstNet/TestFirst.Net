using NUnit.Framework;
using TestFirst.Net.Matcher;

namespace TestFirst.Net.Test.Matcher
{
    [TestFixture]
    public class ABoolTest : BaseMatcherTest
    {
        [Test]
        public void TrueTest()
        {
            AssertPasses(true, ABool.True());

            AssertFails(false, ABool.True());
            AssertFails(null, ABool.True());
        }

        [Test]
        public void FalseTest()
        {
            AssertPasses(false, ABool.False());

            AssertFails(true, ABool.False());
            AssertFails(null, ABool.False());
        }

        [Test]
        public void NullTest()
        {
            AssertPasses(null, ABool.Null());

            AssertFails(true, ABool.Null());
            AssertFails(false, ABool.Null());
        }

        [Test]
        public void NotNullTest()
        {
            AssertPasses(true, ABool.NotNull());
            AssertPasses(false, ABool.NotNull());

            AssertFails(null, ABool.NotNull());
        }

        [Test]
        public void EqualToTest()
        {
            AssertPasses(true, ABool.EqualTo(true));
            AssertPasses(false, ABool.EqualTo(false));

            AssertFails(false, ABool.EqualTo(true));
            AssertFails(true, ABool.EqualTo(false));

            AssertFails(null, ABool.EqualTo(true));
            AssertFails(null, ABool.EqualTo(false));
        }
    }
}
