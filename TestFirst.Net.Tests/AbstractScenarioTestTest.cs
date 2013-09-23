using System;
using NUnit.Framework;
using TestFirst.Net.Matcher;

namespace TestFirst.Net.Test
{
    [TestFixture]
    public class AbstractScenarioTestTest:AbstractScenarioTest
    {
        [Test]
        public void EmptyOrNullTitleFailsTest()
        {
            AssertTitleFails(null);
            AssertTitleFails("");
            AssertTitleFails(" ");
        }
        
        [Test]
        public void TitleOkTest()
        {
            Scenario("MyTitle").GivenNothing().WhenNothing().Then(true, ABool.True());
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
            Assert.NotNull(thrown,"Expect assertion failed exception to be thrown");
        }

        [Test]
        public void DefaultInjectorSetTest()
        {
            String myString;

            Scenario("DefaultInjectorSetTest")
                .Given(myString = Get(MyStringFetcher.With("123")))
                .WhenNothing()
                .Then(Expect(myString), Is(AString.EqualTo("123")));
        }

        class MyStringFetcher : IFetcher<String>
        {
            private String m_val;

            internal static MyStringFetcher With(String val)
            {
                return new MyStringFetcher {m_val = val};
            }

            public String Fetch()
            {
                return m_val;
            }
        }

    }
}
