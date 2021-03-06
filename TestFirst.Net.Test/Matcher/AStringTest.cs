using NUnit.Framework;
using TestFirst.Net.Matcher;

namespace TestFirst.Net.Test.Matcher
{
    [TestFixture]
    public class AStringTest : BaseMatcherTest
    {
        [Test]
        public void NotTest()
        {
            AssertPasses("Two", AString.Not(AString.EqualTo("One")));
            AssertFails("One", AString.Not(AString.EqualTo("One")));
            AssertPasses("one", AString.Not(AString.EqualTo("One")));
        }

        [Test]
        public void EqualsToTest()
        {
            AssertPasses("One", AString.EqualTo("One"));

            AssertFails("one", AString.EqualTo("One"));
            AssertFails(string.Empty, AString.EqualTo("One"));
            AssertFails(null, AString.EqualTo("One"));
        }

        [Test]
        public void EqualsToNullTest()
        {
            string nil = null;
            AssertPasses(nil, AString.EqualTo(nil));
            AssertFails("one", AString.EqualTo(nil));
            AssertFails(string.Empty, AString.EqualTo(nil));
        }

        [Test]
        public void NullTest()
        {
            AssertPasses(null, AString.Null());
            AssertFails("one", AString.Null());
            AssertFails(string.Empty, AString.Null());
        }

        [Test]
        public void NotNullTest()
        {
            AssertFails(null, AString.NotNull());
            AssertPasses("one", AString.NotNull());
            AssertPasses(string.Empty, AString.NotNull());
        }

        [Test]
        public void EqualsToIgnoreCaseTest()
        {
            AssertPasses("One", AString.EqualToIgnoringCase("One"));
            AssertPasses("ONE", AString.EqualToIgnoringCase("One"));

            AssertFails(string.Empty, AString.EqualToIgnoringCase("One"));
            AssertFails(null, AString.EqualToIgnoringCase("One"));
        }

        [Test]
        public void ContainsTest()
        {
            AssertPasses("cat", AString.Containing("cat"));
            AssertPasses("the cat sat on the mat", AString.Containing("cat"));

            AssertFails("the dog sat on the mat", AString.Containing("cat"));
            AssertFails("the CAT sat on the mat", AString.Containing("cat"));

            AssertFails(string.Empty, AString.Containing("cat"));
            AssertFails("ca", AString.Containing("cat"));
            AssertFails(null, AString.Containing("cat"));
        }

        [Test]
        public void ContainsIgnoreCaseTest()
        {
            AssertPasses("cat", AString.ContainingOfAnyCase("cat"));
            AssertPasses("CAT", AString.ContainingOfAnyCase("cat"));

            AssertPasses("the cat sat on the mat", AString.ContainingOfAnyCase("cat"));
            AssertPasses("the CAT sat on the mat", AString.ContainingOfAnyCase("cat"));
            
            AssertFails("the dog sat on the mat", AString.ContainingOfAnyCase("cat"));

            AssertFails(string.Empty, AString.ContainingOfAnyCase("cat"));
            AssertFails("ca", AString.ContainingOfAnyCase("cat"));
            AssertFails(null, AString.ContainingOfAnyCase("cat"));
        }

        [Test]
        public void TrimmedLengthTest()
        {
            AssertPasses(null, AString.TrimmedLength(AnInt.EqualTo(0)));
            AssertPasses(string.Empty, AString.TrimmedLength(AnInt.EqualTo(0)));
            AssertPasses(" ", AString.TrimmedLength(AnInt.EqualTo(0)));
            AssertPasses(" a ", AString.TrimmedLength(AnInt.EqualTo(1)));
            AssertPasses(" a b ", AString.TrimmedLength(AnInt.EqualTo(3)));

            AssertFails(null, AString.TrimmedLength(AnInt.EqualTo(1)));
            AssertFails(string.Empty, AString.TrimmedLength(AnInt.EqualTo(1)));
            AssertFails(" ", AString.TrimmedLength(AnInt.EqualTo(1)));
            AssertFails(" a ", AString.TrimmedLength(AnInt.EqualTo(0)));
            AssertFails(" a ", AString.TrimmedLength(AnInt.EqualTo(2)));
            AssertFails(" a b ", AString.TrimmedLength(AnInt.EqualTo(1)));
        }

        [Test]
        public void EqualsItemsTest()
        {
            AssertPasses(Items("one"), AString.EqualToValues("one"));
            AssertPasses(Items("one", "two", "three"), AString.EqualToValues("one", "two", "three"));

            AssertFails(Items("one"), AString.EqualToValues("zero"));
            AssertFails(Items("one", "two", "three"), AString.EqualToValues("zero", "one", "two"));
        }

        [Test]
        public void AsIntTest()
        {
            AssertPasses("0", AString.As(AnInt.EqualTo(0)));
            AssertPasses("1", AString.As(AnInt.EqualTo(1)));
            AssertPasses("999", AString.As(AnInt.GreaterThan(50)));

            AssertFails(null, AString.As(AnInt.EqualTo(0)));
            AssertFails(string.Empty, AString.As(AnInt.EqualTo(0)));
            AssertFails(" ", AString.As(AnInt.EqualTo(0)));
            AssertFails("zero", AString.As(AnInt.EqualTo(0)));
        }

        [Test]
        public void StartsWithTest()
        {
            AssertPasses("MyString", AString.StartingWith("MyString"));
            AssertPasses("MyString", AString.StartingWith("My"));

            AssertFails(string.Empty, AString.StartingWith("MyString"));
            AssertFails(null, AString.StartingWith("MyString"));
            AssertFails("MyString", AString.StartingWith("Other"));
        }

        [Test]
        public void EndsWithTest()
        {
            AssertPasses("MyString", AString.EndingWith("MyString"));
            AssertPasses("MyString", AString.EndingWith("String"));

            AssertFails(string.Empty, AString.EndingWith("MyString"));
            AssertFails(null, AString.EndingWith("MyString"));
            AssertFails("MyString", AString.EndingWith("Other"));
        }

        [Test]
        public void Blank()
        {
            AssertPasses(string.Empty, AString.Blank());
            AssertPasses(" ", AString.Blank());
            AssertPasses(" \t", AString.Blank());
            AssertPasses(null, AString.Blank());

            AssertFails("x", AString.Blank());
        }

        [Test]
        public void Empty()
        {
            AssertPasses(string.Empty, AString.Empty());

            AssertFails(" ", AString.Empty());
            AssertFails("\t", AString.Empty());
            AssertFails(null, AString.Empty());
            AssertFails("x", AString.Empty());
        }

        [Test]
        public void EmptyOrNull()
        {
            AssertPasses(string.Empty, AString.EmptyOrNull());
            AssertPasses(null, AString.EmptyOrNull());

            AssertFails(" ", AString.EmptyOrNull()); 
            AssertFails(" \t", AString.EmptyOrNull());
            AssertFails("x", AString.EmptyOrNull());
        }
    }
}
