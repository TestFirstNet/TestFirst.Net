using NUnit.Framework;
using TestFirst.Net.Matcher;

namespace TestFirst.Net.Test
{
    [TestFixture]
    public class ScenarioTest : BaseScenarioTest
    {
        [Test]
        public void GivenAction_whenAction_thenAction()
        {
            var chain = new CallChainAssert();
            new Scenario()
                .Given(() => chain.Called("given"))
                .When(() => chain.Called("when"))
                .Then(() => chain.Called("then"));

            chain.AssertCalledInOrder("given,when,then");
        }

        [Test]
        public void GivenMultipleActions_whenAction_thenAction()
        {
            var chain = new CallChainAssert();
            new Scenario()
                .Given(() => chain.Called("given1"))
                .Given(() => chain.Called("given2"))
                .Given(() => chain.Called("given3"))
                .When(() => chain.Called("when"))
                .Then(() => chain.Called("then"));

            chain.AssertCalledInOrder("given1,given2,given3,when,then");
        }

        [Test]
        public void GivenAction_whenMultipleActions_thenAction()
        {
            var chain = new CallChainAssert();
            new Scenario()
                .Given(() => chain.Called("given"))
                .When(() => chain.Called("when1"))
                .When(() => chain.Called("when2"))
                .When(() => chain.Called("when3"))
                .Then(() => chain.Called("then"));

            chain.AssertCalledInOrder("given,when1,when2,when3,then");
        }

        [Test]
        public void MultipleThens_areInvoked()
        {
            var chain = new CallChainAssert();
            new Scenario()
                .Given(() => chain.Called("given"))
                .When(() => chain.Called("when"))
                .Then(() => chain.Called("then1"))
                .Then(() => chain.Called("then2"));

            chain.AssertCalledInOrder("given,when,then1,then2");
        }

        [Test]
        public void MultipleGivens_areInvoked()
        {
            var chain = new CallChainAssert();
            new Scenario()
                .Given(() => chain.Called("given1"))
                .Given(() => chain.Called("given2"))
                .When(() => chain.Called("when"))
                .Then(() => chain.Called("then"));

            chain.AssertCalledInOrder("given1,given2,when,then");
        }


        [Test]
        public void MultipleWhens_areInvoked()
        {
            var chain = new CallChainAssert();
            new Scenario()
                .Given(() => chain.Called("given"))
                .When(() => chain.Called("when1"))
                .When(() => chain.Called("when2"))
                .Then(() => chain.Called("then"));

            chain.AssertCalledInOrder("given,when1,when2,then");
        }

        [Test]
        public void SubsequentFailAfterPassThenFailsTest()
        {
            var whenStep = new Scenario().Given(DoNothingAction()).When(DoNothingAction());

            AssertFails(() => whenStep
                                .Then(() => "ok", AString.EqualTo("ok"))
                                .Then(() => "", AString.EqualTo("fail")));
        }

        [Test]
        public void SubsequentPassThenDoesNotResetPreviousFailTest()
        {

            var whenStep = new Scenario().Given(DoNothingAction()).When(DoNothingAction());

            AssertFails(() => whenStep
                                .Then(() => "", AString.EqualTo("fail"))
                                //next call should not reset scenario to pass
                                .Then(() => "ok", AString.EqualTo("ok")));
        }

    }
}
