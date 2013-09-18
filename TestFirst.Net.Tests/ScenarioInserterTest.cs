using System;
using NUnit.Framework;

namespace TestFirst.Net.Test
{
    [TestFixture]
    public class ScenarioInserterTest : BaseScenarioTest
    {
        [Test]
        public void GivenInserter_whenNone_thenAction()
        {
            var chain = new CallChainAssert();
            new Scenario()
                .Given(InserterWithMsg(chain,"insert"))
                .Then(ActionWithMsg(chain,"then"));

            chain.AssertCalledInOrder("insert,then");
        }

        [Test]
        public void GivenInserter_whenAction_thenAction()
        {
            var chain = new CallChainAssert();
            new Scenario()
                .Given(InserterWithMsg(chain, "insert"))
                .When(ActionWithMsg(chain,"when"))
                .Then(ActionWithMsg(chain, "then"));

            chain.AssertCalledInOrder("insert,when,then");
        }

        Action ActionWithMsg(CallChainAssert chain, string msg)
        {
            return new Action(() => chain.Called(msg));
        }

        IInserter InserterWithMsg(CallChainAssert chain,string msg)
        {
            return new ActionInvokeInserter(()=>chain.Called(msg));
        }

        private class ActionInvokeInserter : IInserter
        {
            private readonly Action m_action;

            public ActionInvokeInserter(Action action)
            {
                m_action = action;
            }

            public void Insert()
            {
                m_action.Invoke();
            }
        }
    }
}
