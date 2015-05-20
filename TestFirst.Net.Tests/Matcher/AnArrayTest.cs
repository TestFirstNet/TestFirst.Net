using NUnit.Framework;
using TestFirst.Net.Matcher;
using System;

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
            AssertPasses(new String[] { "Alice","Bob","Tim" }, AnArray.EqualTo(new String[] { "Alice", "Bob", "Tim" }));

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

        [Test]
        public void NotEmpty()
        {
            AssertPasses(new string[] { "1" }, AnArray.NotEmpty<string>());
            AssertPasses(new int?[] { 1 }, AnArray.NotEmpty<int?>());

            AssertFails(null, AnArray.NotEmpty<string>());
            AssertFails(null, AnArray.NotEmpty<int?>());

            AssertFails(new string[] {}, AnArray.NotEmpty<string>());
            AssertFails(new int?[] {}, AnArray.NotEmpty<int?>());
        }

        [Test]
        public void Empty()
        {
            AssertPasses(new string[] {}, AnArray.Empty<string>());
            AssertPasses(new int?[] {}, AnArray.Empty<int?>());

            AssertFails(new string[] { "1" }, AnArray.Empty<string>());
            AssertFails(new int?[] { 1 }, AnArray.Empty<int?>());

            AssertFails(null, AnArray.Empty<string>());
            AssertFails(null, AnArray.Empty<int?>());
        }

        [Test]
        public void EmptyOrNull()
        {
            AssertPasses(new string[] {}, AnArray.EmptyOrNull<string>());
            AssertPasses(new int?[] {}, AnArray.EmptyOrNull<int?>());

            AssertPasses(null, AnArray.EmptyOrNull<string>());
            AssertPasses(null, AnArray.EmptyOrNull<int?>());

            AssertFails(new string[] { "1" }, AnArray.EmptyOrNull<string>());
            AssertFails(new int?[] { 1 }, AnArray.EmptyOrNull<int?>());
        }

        private static byte[] RandomBytes()
        {
            return new TestFirst.Net.Random.Random().Bytes();
        }
    }
}
