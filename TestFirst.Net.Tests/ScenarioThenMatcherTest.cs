using NUnit.Framework;
using TestFirst.Net.Matcher;

namespace TestFirst.Net.Test
{
    [TestFixture]
    public class ScenarioThenMatcherTest : BaseScenarioTest
    {
        [Test]
        public void Scenario_givenAction_whenFunction_thenMatcher()
        {
            //passing
            new Scenario()
                .Given(DoNothingAction())
                .Then("foo",AString.EqualTo("foo"));

            //failing
            AssertFails(() =>
                 new Scenario()
                    .Given(DoNothingAction())
                    .Then("foo",AString.EqualTo("not foo"))
            );
        }

    }
}
