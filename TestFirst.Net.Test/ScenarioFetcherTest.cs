using NUnit.Framework;
using TestFirst.Net.Matcher;

namespace TestFirst.Net.Test
{
    [TestFixture]
    public class ScenarioFetcherTest : BaseScenarioTest
    {
        [Test]
        public void GivenAction_whenAction_thenFetcherAndMatcher()
        {
            // passing
            new Scenario("using WhenStep, THEN fetch+match passes")
                .Given(DoNothingAction())
                .When(DoNothingAction())
                .Then(WhenFetch("foo"), AString.EqualTo("foo"));

            // failing
            AssertFails(() =>
                new Scenario("using WhenStep, THEN fetch+match fails")
                    .Given(DoNothingAction())
                    .When(DoNothingAction())
                    .Then(WhenFetch("foo"), AString.EqualTo("not foo")));
        }

        [Test]
        public void GivenAction_whenFunction_thenFetcherAndMatcher()
        {
            // passing
            new Scenario("using WhenStep<T>, THEN fetch+match passes")
                .Given(DoNothingAction())
                .When(FuncReturn("a string"))
                .Then(WhenFetch("foo"), AString.EqualTo("foo"));

            // failing
            AssertFails(() =>
                new Scenario("using WhenStep<T>, THEN fetch+match fails")
                    .Given(DoNothingAction())
                    .When(FuncReturn("a string"))
                    .Then(WhenFetch("foo"), AString.EqualTo("not foo")));
        }

        private IFetcher<T> WhenFetch<T>(T instance)
        {
            return new ReturnInstanceFetcher<T>(instance);
        } 

        private class ReturnInstanceFetcher<T> : IFetcher<T>
        {
            private readonly T m_instance;

            public ReturnInstanceFetcher(T instance)
            {
                m_instance = instance;
            }

            public T Fetch()
            {
                return m_instance;
            }
        }
    }
}
