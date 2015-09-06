using System;

namespace Matcher
{
    [TestFixture]
    public class MatchersTest : BaseMatcherTest
    {
        [Test]
        public void NotTest()
        {
            AssertPasses("Two", AString.Not(AString.EqualTo("One")));
            AssertFails("One", AString.Not(AString.EqualTo("One")));
            AssertPasses("one", AString.Not(AString.EqualTo("One")));
        }
    }
}

