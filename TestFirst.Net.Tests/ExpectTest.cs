using System;
using NUnit.Framework;
using TestFirst.Net.Matcher;
using TestFirst.Net.Test.Matcher;

namespace TestFirst.Net
{
    [TestFixture]
    public class ExpectTest : BaseMatcherTest
    {
        [Test]
        public void That_Is_Matcher()
        {
            AssertPasses(()=>Expect.That("x").Is (AString.EqualTo ("x")));
            AssertFails(()=>Expect.That("x").Is (AString.EqualTo ("y")));
        }

        [Test]
        public void For_That_Is_Matcher()
        {
            AssertPasses(()=>Expect.For("myMsg").That("x").Is (AString.EqualTo ("x")));
            AssertFails(()=>Expect.For("myMsg").That("x").Is (AString.EqualTo ("y")));
        }

        [Test]
        public void That_IsEqualTo()
        {
            AssertPasses(()=>Expect.That("x").IsEqualTo("x"));
            AssertFails(()=>Expect.That("x").IsEqualTo("y"));
        }

        [Test]
        public void For_That_IsEqualTo()
        {
            AssertPasses(()=>Expect.For("myMsg").That("x").IsEqualTo("x"));
            AssertFails(()=>Expect.For("myMsg").That("x").IsEqualTo("y"));
        }

        [Test]
        public void That_IsX_And()
        {
            AssertPasses(() => Expect.That("x").IsEqualTo("x").And(AString.EqualTo("x")));
            AssertFails(() => Expect.That("x").IsEqualTo("x").And(AString.EqualTo("y")));
        }

        [Test]
        public void That_IsX_And_With_Nullable_Matcher()
        {
            AssertPasses(() => Expect.That(1).IsEqualTo(1).And(AnInt.EqualTo(1)));
            AssertFails(() => Expect.That(1).IsEqualTo(1).And(AnInt.EqualTo(2)));
        }
        [Test]
        public void That_IsNull()
        {
            Object x = null;
            AssertPasses(()=>Expect.That(x).IsNull());
            AssertFails(()=>Expect.That("x").IsNull());
        }

        [Test]
        public void For_That_IsNull()
        {
            Object x = null;
            AssertPasses(()=>Expect.For("myMsg").That(x).IsNull());
            AssertFails(()=>Expect.For("myMsg").That("x").IsNull());
        }

        [Test]
        public void That_IsNotNull()
        {
            Object x = null;
            AssertPasses(()=>Expect.That("x").IsNotNull());
            AssertFails(()=>Expect.That(x).IsNotNull());
        }

        [Test]
        public void For_That_IsNotNull()
        {
            Object x = null;
            AssertPasses(()=>Expect.For("myMsg").That("x").IsNotNull());
            AssertFails(()=>Expect.For("myMsg").That(x).IsNotNull());
        }

        [Test]
        public void That_Throws()
        {
            AssertPasses(()=>Expect.That(()=>{throw new Exception("SomeError");}).Throws(AnException.With().Message("SomeError")));
            AssertFails(()=>Expect.That(()=>{throw new Exception("Wrong Error Msg");}).Throws(AnException.With().Message("SomeError")));
        }

        [Test]
        public void For_That_Throws()
        {
            AssertPasses(()=>Expect.For("myMsg").That(()=>{throw new Exception("SomeError");}).Throws(AnException.With().Message("SomeError")));
            AssertFails(()=>Expect.For("myMsg").That(()=>{throw new Exception("Wrong Error Msg");}).Throws(AnException.With().Message("SomeError")));
        }


    }
}

