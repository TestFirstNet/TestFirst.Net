using NUnit.Framework;
using TestFirst.Net.Matcher;

namespace TestFirst.Net.Test
{
    [TestFixture]
    public class AbstractScenarioTestTest : AbstractScenarioTest
    {
        [Test]
        public void EmptyOrNullTitleFailsTest()
        {
            AssertTitleFails(null);
            AssertTitleFails(string.Empty);
            AssertTitleFails(" ");
        }
        
        [Test]
        public void TitleOkTest()
        {
            Scenario("MyTitle").GivenNothing().WhenNothing().Then(true, ABool.True());
        }

        [Test]
        public void DefaultInjectorSetTest()
        {
            string myString;

            Scenario("DefaultInjectorSetTest")
                .Given(myString = Get(MyStringFetcher.With("123")))
                .WhenNothing()
                .Then(Expect(myString), Is(AString.EqualTo("123")));
        }

        private void AssertTitleFails(string title)
        {
            AssertionFailedException thrown = null;
            try
            {
                Scenario(title);
            }
            catch (AssertionFailedException e)
            {
                thrown = e;
            }
            Assert.NotNull(thrown, "Expect assertion failed exception to be thrown");
        }

        internal class MyStringFetcher : IFetcher<string>
        {
            private string m_val;

            public string Fetch()
            {
                return m_val;
            }

            internal static MyStringFetcher With(string val)
            {
                return new MyStringFetcher { m_val = val };
            }
        }
    }
}
