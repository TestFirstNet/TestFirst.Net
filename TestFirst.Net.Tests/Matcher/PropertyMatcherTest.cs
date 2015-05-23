using System;
using System.Collections.Generic;
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
        public void MissMatchingPropertyMessageTest()
        {
            AssertFails(
                new FooPoco() { StringProp = "MyString"},
                ExpectFoo()
                    .WithProperty("StringProp", AString.EqualTo("MyWrongString")),
                All.Of(
                    AString.Containing("property:StringProp"),
                    AString.Containing("the string 'MyWrongString'")
                )
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

        [Test]
        //for a bug raised where List matcher becomes confused with property matchers for
        //classes which have subclasses
        public void CanSubClassMatcherAndMatchOnSuperTypePocoTest()
        {
            IList<Employee> employees = new List<Employee>() { new Employee("Bob") };
            IList<Human> humans = new List<Human>() { new Human("Bob") };

            AssertPasses(
                humans,
                AList.InOrder().WithOnly(AHuman.With().Name("Bob"))
                );
            
            AssertPasses(
                employees,
                AList.InOrder().WithOnly(AnEmployee.With().Name("Bob"))
                );

            AssertPasses(
                employees,
                AList.InOrder().WithOnly(AHuman.With().Name("Bob"))
                );  
        }

        [Test]
        public void CanMatchOnSuperClassPropertyTest()
        {
            AssertPasses(
                new Employee("Bob"),
                AnEmployeeNotSubHumanMather.With().Name("Bob")
            );
         
            AssertFails(
                new Employee("NotBob"),
                AnEmployeeNotSubHumanMather.With().Name("Bob")
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
            public List<Guid> Guids { get; set;  }
        }


        internal class Employee : Human
        {
            
            public Employee(String name):base(name) {

            }
        }

        internal class Human
        {
            public Human(String name){
                Name = name;
            }

            public String Name { get; set; }

        }

        internal class AHuman<TSelf,T> : PropertyMatcher<T> 
            where TSelf : AHuman<TSelf, T>
            where T : Human
        {
            private static readonly Human PropertyNames = null;

            public TSelf Name(string name)
            {
                WithProperty(() => PropertyNames.Name, AString.EqualTo(name));
                return (TSelf)this;
            }
        }
        internal class AHuman : AHuman<AHuman,Human>
        {
            public static AHuman With()
            {
                return new AHuman();
            }
        }

        internal class AnEmployee : AHuman<AnEmployee,Employee>
        {
            private static readonly Employee PropertyNames = null;

            public static AnEmployee With()
            {
                return new AnEmployee();
            }

        }

        internal class AnEmployeeNotSubHumanMather : PropertyMatcher<Employee>
        {
            private static readonly Employee PropertyNames = null;

            public static AnEmployeeNotSubHumanMather With()
            {
                return new AnEmployeeNotSubHumanMather();
            }

            public AnEmployeeNotSubHumanMather Name(string name)
            {
                WithProperty(() => PropertyNames.Name, AString.EqualTo(name));
                return this;
            }
        }
    }
}
