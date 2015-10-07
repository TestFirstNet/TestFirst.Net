using System;
using NUnit.Framework;
using TestFirst.Net.Matcher;

namespace TestFirst.Net.Test.Matcher
{
    [TestFixture]
    public class AListInOrderTest : BaseMatcherTest
    {
        [Test]
        public void InOrderExactPassesTest()
        {
            AssertPasses(
                Items("one", "two", "three"), 
                AList.InOrder().WithOnly(AString.EqualToValues("one", "two", "three")));
        }

        [Test]
        public void InOrderExactOutOfOrderItemsFailsTest()
        {
            AssertFails(
                Items("two", "one", "three"),
                AList.InOrder().WithOnly(AString.EqualToValues("one", "two", "three")));
        }

        [Test]
        public void InOrderExactAdditionalItemFailsTest()
        {
            AssertFails(
                Items("one", "two", "three", "four"),
                AList.InOrder().WithOnly(AString.EqualToValues("one", "two", "three")));
        }

        [Test]
        public void InOrderExactMissingItemFailsTest()
        {
            AssertFails(
                Items("one", "two"),
                AList.InOrder().WithOnly(AString.EqualToValues("one", "two", "three")));
        }

        [Test]
        public void InOrderAtLeastPrimitiveIntItemsPass()
        {
            AssertPasses(
                Items(1, 2, 3),
                AList.InOrder().WithAtLeast(AnInt.EqualTo(1)));
        }

        [Test]
        public void InOrderAtLeastPrimitiveGuidItemsPass()
        {
            System.Collections.Generic.IEnumerable<Guid> list = new System.Collections.Generic.List<Guid>();

            if (!typeof(System.Collections.Generic.IEnumerable<Guid>).IsInstanceOfType(list)) 
            {
                throw new ArgumentException("types don't match");
            }
            Guid g = Guid.NewGuid();
            AssertPasses(
                Items(g, Guid.NewGuid()),
                AList.InOrder().WithAtLeast(AGuid.EqualTo(g)));
        }

        [Test]
        public void InOrderAtLeastPassesTest()
        {
            AssertPasses(
                Items("one", "two", "three"),
                AList.InOrder().WithAtLeast(AString.EqualToValues("one", "two", "three")));
        }

        [Test]
        public void InOrderAtLeastAdditionalItemAtEndPassesTest()
        {
            AssertPasses(
                Items("one", "two", "three", "four"),
                AList.InOrder().WithAtLeast(AString.EqualToValues("one", "two", "three")));
        }

        [Test]
        public void InOrderAtLeastAdditionalItemAtStartPassesTest()
        {
            AssertPasses(
                Items("zero", "one", "two", "three"),
                AList.InOrder().WithAtLeast(AString.EqualToValues("one", "two", "three")));
        }

        [Test]
        public void InOrderAtLeastAdditionalItemAtMiddlePassesTest()
        {
            AssertPasses(
                Items("one", "and a half", "two", "three"),
                AList.InOrder().WithAtLeast(AString.EqualToValues("one", "two", "three")));
        }

        [Test]
        public void InOrderAtLeastAdditionalItemAtStartAndEndPassesTest()
        {
            AssertPasses(
                Items("zero", "one", "two", "three", "four"),
                AList.InOrder().WithAtLeast(AString.EqualToValues("one", "two", "three")));
        }

        [Test]
        public void InOrderAtLeastDuplicateItemAtEndPassesTest()
        {
            AssertPasses(
                Items("one", "two", "three", "three"),
                AList.InOrder().WithAtLeast(AString.EqualToValues("one", "two", "three")));
        }

        [Test]
        public void InOrderAtLeastDuplicateItemAtStartPassesTest()
        {
            AssertPasses(
                Items("one", "one", "two", "three", "three"),
                AList.InOrder().WithAtLeast(AString.EqualToValues("one", "two", "three")));
        }

        [Test]
        public void InOrderAtLeastMissingItemAtStartFailsTest()
        {
            AssertFails(
                Items("two", "three"),
                AList.InOrder().WithAtLeast(AString.EqualToValues("one", "two", "three")));
        }

        [Test]
        public void InOrderAtLeastMissingItemAtEndFailsTest()
        {
            AssertFails(
                Items("one", "two"),
                AList.InOrder().WithAtLeast(AString.EqualToValues("one", "two", "three")));
        }

        [Test]
        public void InOrderAtLeastMissingItemMiddleFailsTest()
        {
            AssertFails(
                Items("one", "three"),
                AList.InOrder().WithAtLeast(AString.EqualToValues("one", "two", "three")));
        }

        [Test]
        public void InOrderAtLeastOutOfOrderItemsFailsTest()
        {
            AssertFails(
                Items("two", "one", "three"),
                AList.InOrder().WithAtLeast(AString.EqualToValues("one", "two", "three")));
        }

        [Test]
        public void ArrayInOrderExactPassesTest()
        {
            AssertPasses(
                Array("one", "two", "three"), 
                AList.InOrder().WithOnly(AString.EqualToValues("one", "two", "three")));
        }

        [Test]
        public void ArrayInOrderExactOutOfOrderItemsFailsTest()
        {
            AssertFails(
                Array("two", "one", "three"),
                AList.InOrder().WithOnly(AString.EqualToValues("one", "two", "three")));
        }

        [Test]
        public void ArrayInOrderAtLeastMissingItemMiddleFailsTest()
        {
            AssertFails(
                Array("one", "three"),
                AList.InOrder().WithAtLeast(AString.EqualToValues("one", "two", "three")));
        }

        [Test]
        public void ArrayInOrderAtLeastOutOfOrderItemsFailsTest()
        {
            AssertFails(
                Array("two", "one", "three"),
                AList.InOrder().WithAtLeast(AString.EqualToValues("one", "two", "three")));
        }
    }
}
