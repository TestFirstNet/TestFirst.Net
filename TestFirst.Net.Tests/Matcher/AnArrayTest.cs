using NUnit.Framework;
using TestFirst.Net.Matcher;

namespace TestFirst.Net.Test.Matcher
{
    [TestFixture]
    public class AnArrayTest : BaseMatcherTest
    {
        [Test]
        public void EqualTo()
        {
            AssertPasses((byte[])null, AnArray.EqualTo((byte[])null));
            AssertPasses(new byte[]{}, AnArray.EqualTo(new byte[]{}));

            AssertFails(new byte[] { }, AnArray.EqualTo((byte[])null));
            AssertFails(null, AByteArray.EqualTo(new byte[] { }));
            AssertFails(new byte[] { 1 }, AnArray.EqualTo(new byte[] { }));
            AssertFails(new byte[] { }, AnArray.EqualTo(new byte[] { 1 }));

            AssertPasses(new byte[] { 1, 2, 3 }, AnArray.EqualTo(new byte[] { 1, 2, 3 }));
            var b = RandomBytes();
            AssertPasses(b, AnArray.EqualTo(b));
        }

        [Test]
        public void Null()
        {
            AssertPasses(null, AnArray.Null<string>());
            AssertPasses(null, AnArray.Null<int?>());

            AssertFails(new string[] { }, AnArray.Null<string>());
            AssertFails(new int?[] { }, AnArray.Null<int?>());
        }

        [Test]
        public void NotNull()
        {
            AssertPasses(new string[] { }, AnArray.NotNull<string>());
            AssertPasses(new int?[] { }, AnArray.NotNull<int?>());

            AssertFails(null, AnArray.NotNull<string>());
            AssertFails(null, AnArray.NotNull<int?>());
        }



        private static byte[] RandomBytes()
        {
            return new TestFirst.Net.Random.Random().Bytes();
        }
    }
}
