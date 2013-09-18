using System;
using NUnit.Framework;
using TestFirst.Net.Matcher;

namespace TestFirst.Net.Test.Matcher
{
    [TestFixture]
    public class AnInstanceTest:BaseMatcherTest
    {
        [Test]
        public void EqualToPassesTest()
        {
            AssertPasses("", AnInstance.EqualTo(""));
            AssertPasses("a", AnInstance.EqualTo("a"));
            AssertPasses(5, AnInstance.EqualTo(5));

            var instance = new Object();
            AssertPasses(instance, AnInstance.EqualTo(instance));
            AssertPasses(SettableEquals.Returns(true), AnInstance.EqualTo(SettableEquals.Returns(true)));
            //actuals Equals method shouldn't be invoked
            AssertPasses(SettableEquals.Returns(false), AnInstance.EqualTo(SettableEquals.Returns(true)));
        }

        [Test]
        public void EqualToFailsTest()
        {
            AssertFails("", AnInstance.EqualTo((string)null));
            AssertFails(null, AnInstance.EqualTo(""));
            AssertFails("a", AnInstance.EqualTo("b"));
            AssertFails(5, AnInstance.EqualTo(6));
            AssertFails(new Object(), AnInstance.EqualTo(new Object()));
            AssertFails(SettableEquals.Returns(true), AnInstance.EqualTo(SettableEquals.Returns(false)));
        }

        [Test]
        public void OfTypeTest()
        {
            AssertPasses("mystring", AnInstance.OfType<String>());
            AssertPasses(new MySubType(), AnInstance.OfType<MySubType>());
            AssertPasses(new MySubType(), AnInstance.OfType<MyType>());
            AssertPasses(new MySubType(), AnInstance.OfType<Object>());
 
            AssertFails(1, AnInstance.OfType<String>());
            AssertFails(new Object(), AnInstance.OfType<MyType>());
        }


        internal class SettableEquals
        {
            private bool EqualsReturns { get; set; }

            #pragma warning disable 659
            public override bool Equals(object obj)
            {
                return EqualsReturns;
            }
            #pragma warning restore 659

            internal static SettableEquals Returns(bool b)
            {
                return new SettableEquals {EqualsReturns = b};
            }
        }

        internal class MyType
        {

        }

        internal class MySubType :MyType
        {

        }
    }
}
