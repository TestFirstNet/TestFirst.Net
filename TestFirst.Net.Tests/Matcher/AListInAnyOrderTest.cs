using NUnit.Framework;
using TestFirst.Net.Matcher;

namespace TestFirst.Net.Test.Matcher
{
    [TestFixture]
    public class AListInAnyOrderTest : BaseMatcherTest
    {
        [Test]
        public void InAnyOrderWithPassesTest()
        {
            AssertPasses(
                Items("one", "two", "three"), 
                AList.InAnyOrder().WithOnly(AString.EqualToValues("one", "two", "three"))
            );
        }

        [Test]
        public void InAnyOrderWithOutOfOrderPassesTest()
        {
            AssertPasses(
                Items("two", "one", "three"),
                AList.InAnyOrder().WithOnly(AString.EqualToValues("one", "two", "three"))
            );
        }

        [Test]
        public void InAnyOrderWithAdditionalItemFailsTest()
        {
            AssertFails(
                Items( "one", "two", "three", "four"),
                AList.InAnyOrder().WithOnly(AString.EqualToValues("one", "two", "three"))
            );
        }

        [Test]
        public void InAnyOrderWithMissingItemFailsTest()
        {
            AssertFails(
                Items("one", "two"),
                AList.InAnyOrder().WithOnly(AString.EqualToValues("one", "two", "three"))
            );
        }

        [Test]
        public void InAnyOrderAtLeastInOrderPassesTest()
        {
            AssertPasses(
                Items("one", "two", "three"),
                AList.InAnyOrder().WithAtLeast(AString.EqualToValues("one", "two", "three"))
            );
        }

        [Test]
        public void InAnyOrderAtLeastOutOfOrderPassesTest()
        {
            AssertPasses(
                Items("three", "one", "two"),
                AList.InAnyOrder().WithAtLeast(AString.EqualToValues("one", "two", "three"))
            );
        }

        [Test]
        public void InAnyOrderAtLeastAdditionalItemPassesTest()
        {
            AssertPasses(
                Items("four", "three", "one", "two"),
                AList.InAnyOrder().WithAtLeast(AString.EqualToValues("one", "two", "three"))
            );
        }

        [Test]
        public void InAnyOrderAtLeastDuplicateItemPassesTest()
        {
            AssertPasses(
                Items("three", "three", "one", "two"),
                AList.InAnyOrder().WithAtLeast(AString.EqualToValues("one", "two", "three"))
            );
        }

        [Test]
        public void InAnyOrderAtLeastOutOfOrderItemsPassesTest()
        {
            AssertPasses(
                Items("two", "one", "three"),
                AList.InAnyOrder().WithAtLeast(AString.EqualToValues("one", "two", "three"))
            );
        }
    }
}
