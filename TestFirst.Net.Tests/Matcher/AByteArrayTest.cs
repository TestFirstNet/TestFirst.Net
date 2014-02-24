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

            AssertFails(new byte[] {}, AByteArray.EqualTo(null));
            AssertFails(null, AByteArray.EqualTo(new byte[] { }));
            AssertFails(new byte[] {1}, AByteArray.EqualTo(new byte[] {}));
            AssertFails(new byte[] {}, AByteArray.EqualTo(new byte[] {1}));

            AssertPasses(new byte[] { 1 }, AByteArray.EqualTo(new byte[] { 1 }));
            var b = RandomBytes();
            AssertPasses(b, AByteArray.EqualTo(b));
        }

        private static byte[] RandomBytes()
        {
            return new TestFirst.Net.Random.Random().Bytes();
        }
    }
}
