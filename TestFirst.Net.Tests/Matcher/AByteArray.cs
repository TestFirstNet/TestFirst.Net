using NUnit.Framework;
using TestFirst.Net.Matcher;

namespace TestFirst.Net.Test.Matcher
{
    [TestFixture]
    public class AByteArrayTest : BaseMatcherTest
    {
        [Test]
        public void EqualTo()
        {
            AssertPasses(null, AByteArray.EqualTo(null));
            AssertPasses(new byte[]{}, AByteArray.EqualTo(new byte[]{}));

            AssertFails(new byte[] { }, AByteArray.EqualTo(null));
            AssertFails(null, AByteArray.EqualTo(new byte[] { }));
            AssertFails(new byte[] { 1 }, AByteArray.EqualTo(new byte[] { }));
            AssertFails(new byte[] { }, AByteArray.EqualTo(new byte[] { 1 }));

            AssertPasses(new byte[] { 1, 2, 3 }, AByteArray.EqualTo(new byte[] { 1, 2, 3 }));

            var b = RandomBytes();
            AssertPasses(b, AnArray.EqualTo(b));
        }

        [Test]
        public void Null()
        {
            AssertPasses(null, AByteArray.Null());

            AssertFails(new byte[] { }, AByteArray.Null());
        }

        [Test]
        public void NotNull()
        {
            AssertPasses(new byte[] { }, AByteArray.NotNull());

            AssertFails(null, AByteArray.NotNull());
        }

        [Test]
        public void NotEmpty()
        {
            AssertPasses(new byte[] { 1 }, AByteArray.NotEmpty());

            AssertFails(null, AByteArray.NotEmpty());

            AssertFails(new byte[] {}, AByteArray.NotEmpty());
        }

        [Test]
        public void Empty()
        {
            AssertPasses(new byte[] {}, AByteArray.Empty());

            AssertFails(new byte[] { 1 }, AByteArray.Empty());

            AssertFails(null, AByteArray.Empty());
        }

        [Test]
        public void EmptyOrNull()
        {
            AssertPasses(new byte[] {}, AByteArray.EmptyOrNull());

            AssertPasses(null, AByteArray.EmptyOrNull());

            AssertFails(new byte[] { 1 }, AByteArray.EmptyOrNull());
        }

        private static byte[] RandomBytes()
        {
            return new Net.Rand.Random().Bytes();
        }
    }
}
