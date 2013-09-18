using System;
using NUnit.Framework;
using TestFirst.Net.Matcher;

namespace TestFirst.Net.Test.Matcher
{
    [TestFixture]
    public class PropertyMatcherTest:BaseMatcherTest
    {
        [Test]
        public void AllPropertiesMatchesTest()
        {
            AssertPasses(
                new FooPoco() { StringProp = "MyString", IntProp = 1 },
                ExpectFoo()
                    .WithProperty("StringProp", AString.EqualTo("MyString"))
                    .WithProperty("IntProp", AnInt.EqualTo(1))
                );
        }

        [Test]
        public void DuplicatePropertiesMatchesTest()
        {
            var actual = new FooPoco() {StringProp = "MyString", IntProp = 1};

            AssertPasses(
                actual,
                ExpectFoo()
                    .WithProperty("StringProp", AString.EqualTo("MyString"))
                    .WithProperty("StringProp", AString.EndingWith("String"))
                    .WithProperty("StringProp", AString.StartingWith("My"))
                );

            AssertFails(
                actual,
                ExpectFoo()
                    .WithProperty("StringProp", AString.EqualTo("MyString"))
                    .WithProperty("StringProp", AString.EndingWith("String"))
                    //ensure this one is also used, not just ignored
                    .WithProperty("StringProp", AString.StartingWith("NotMatching!"))
                );
        }

        [Test]
        public void MissMatchingPropertyTest()
        {
            AssertFails(
                new FooPoco() { StringProp = "MyString", IntProp = 1 },
                ExpectFoo()
                    .WithProperty("StringProp", AString.EqualTo("MyWrongString"))
                    .WithProperty("IntProp", AnInt.EqualTo(1))
                );
        }

        [Test]
        public void PartialPropertiesMatchTest()
        {
            AssertPasses(
                new FooPoco() { StringProp = "MyString", IntProp = 1 },
                ExpectFoo()
                    .WithProperty("StringProp", AString.EqualTo("MyString"))
                );
        }

        [Test]
        public void NullablePropertyMatchTest()
        {
            AssertPasses(
                new FooPoco() { StringProp = null, IntProp = 1 },
                ExpectFoo()
                    .WithProperty("StringProp", AString.EqualTo(null))
                );
        }

        [Test]
        public void MissingPropertyTest()
        {
            bool thrown = false;
            try
            {
                ExpectFoo().WithProperty("NonMatchingProperty", AString.EqualTo("MyString"));
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(e.Message.Contains("Property with name 'NonMatchingProperty' does not exist"));

                thrown = true;
            }
            Assert.IsTrue(thrown, "Expected argument exception on non existing property");
        }

        [Test]
        public void PublicPropertiesTest()
        {
            AssertPasses(
                new FooPoco() { PublicProp = "MyString" },
                ExpectFoo()
                    .WithProperty("PublicProp", AString.EqualTo("MyString"))
                );
        }

        private static PropertyMatcherExposedForTesting<FooPoco> ExpectFoo()
        {
            return new PropertyMatcherExposedForTesting<FooPoco>();
        } 

        private class PropertyMatcherExposedForTesting<T>:PropertyMatcher<T>
        {
            protected internal new PropertyMatcherExposedForTesting<T> WithProperty<TPropertyType>(string propertyName, IMatcher<TPropertyType> fieldMatcher)
            {
                return (PropertyMatcherExposedForTesting<T>) base.WithProperty(propertyName, fieldMatcher);
            }
        }

        internal class FooPoco
        {
            internal String StringProp { get; set; }
            internal int IntProp { get; set; }
            public String PublicProp { get; set; }
        } 
    }
}
