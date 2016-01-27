using System;
using NUnit.Framework;
using TestFirst.Net.Matcher;

namespace TestFirst.Net.Test.Matcher
{
    [TestFixture]
    public class AnInstanceTest : BaseMatcherTest
    {
        [Test]
        public void EqualToPassesTest()
        {
            AssertPasses(string.Empty, AnInstance.EqualTo(string.Empty));
            AssertPasses("a", AnInstance.EqualTo("a"));
            AssertPasses(5, AnInstance.EqualTo(5));

            var instance = new object();
            AssertPasses(instance, AnInstance.EqualTo(instance));
            AssertPasses(SettableEquals.Returns(true), AnInstance.EqualTo(SettableEquals.Returns(true)));

            // actuals Equals method shouldn't be invoked
            AssertPasses(SettableEquals.Returns(false), AnInstance.EqualTo(SettableEquals.Returns(true)));
        }

        [Test]
        public void EqualToFailsTest()
        {
            AssertFails(string.Empty, AnInstance.EqualTo((string)null));
            AssertFails(null, AnInstance.EqualTo(string.Empty));
            AssertFails("a", AnInstance.EqualTo("b"));
            AssertFails(5, AnInstance.EqualTo(6));
            AssertFails(new object(), AnInstance.EqualTo(new object()));
            AssertFails(SettableEquals.Returns(true), AnInstance.EqualTo(SettableEquals.Returns(false)));
        }

        [Test]
        public void OfTypeTest()
        {
            AssertPasses("mystring", AnInstance.OfType<string>());
            AssertPasses(new MySubType(), AnInstance.OfType<MySubType>());
            AssertPasses(new MySubType(), AnInstance.OfType<MyType>());
            AssertPasses(new MySubType(), AnInstance.OfType<object>());
 
            AssertFails(1, AnInstance.OfType<string>());
            AssertFails(new object(), AnInstance.OfType<MyType>());
        }

#pragma warning disable 659
        internal class SettableEquals
        {
            private bool EqualsReturns { get; set; }

            public override bool Equals(object obj)
            {
                return EqualsReturns;
            }

            internal static SettableEquals Returns(bool b)
            {
                return new SettableEquals { EqualsReturns = b };
            }
        }
#pragma warning restore 659

        internal class MyType
        {
        }

        internal class MySubType : MyType
        {
        }
    }
}