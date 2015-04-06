using System;
using NUnit.Framework;
using TestFirst.Net.Matcher;

namespace TestFirst.Net.Test.Matcher
{
    [TestFixture]
    public class AGuidTest : BaseMatcherTest
    {
        [Test]
        public void EqualToTest()
        {
            var guid = Guid.NewGuid();             
            AssertPasses(guid, AGuid.EqualTo(guid));
            AssertPasses(Guid.Parse(guid.ToString()), AGuid.EqualTo(guid));
            AssertPasses(Guid.Empty, AGuid.EqualTo(Guid.Empty));

            AssertFails(null, AGuid.EqualTo(Guid.NewGuid()));
            AssertFails(Guid.NewGuid(), AGuid.EqualTo(Guid.NewGuid()));
            AssertFails(Guid.Empty, AGuid.EqualTo(Guid.NewGuid()));
        }

        [Test]
        public void NotEqualToTest()
        {
            var guid = Guid.NewGuid();
        
            AssertPasses(guid, AGuid.NotEqualTo(Guid.NewGuid()));
            AssertPasses(guid, AGuid.NotEqualTo(Guid.Empty));
            AssertPasses(null, AGuid.NotEqualTo(Guid.NewGuid()));
            AssertPasses(guid, AGuid.NotEqualTo(null));

            AssertFails(null, AGuid.NotEqualTo(null));
            AssertFails(guid, AGuid.NotEqualTo(guid));
        }

        [Test]
        public void NotEmptyTest()
        {
            AssertPasses(Guid.NewGuid(), AGuid.NotEmpty());
            AssertFails(Guid.Empty, AGuid.NotEmpty());
        }

        [Test]
        public void NullTest()
        {
            AssertPasses(null, AGuid.Null());

            AssertFails(Guid.Empty, AGuid.Null());
            AssertFails(Guid.NewGuid(), AGuid.Null());
        }


        [Test]
        public void NotNullTest()
        {
            AssertPasses(Guid.NewGuid(), AGuid.NotNull());
            AssertPasses(Guid.Empty, AGuid.NotNull());

            AssertFails(null, AGuid.NotNull());
        }
    }
}
