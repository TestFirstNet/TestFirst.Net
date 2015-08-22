using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace TestFirst.Net.Test
{
    public abstract class BaseScenarioTest : ScenarioFluency
    {
        protected Scenario Scenario()
        {
            CurrentScenario = new Scenario();
            return CurrentScenario;
        }

        protected Action DoNothingAction()
        {
            return () => { /*Do nothing*/ };
        }

        protected Func<T> FuncReturn<T>(T instance)
        {
            return () => instance;
        }

        protected void AssertFails(Action action)
        {
            AssertionFailedException exception = null;
            try
            {
                action.Invoke();
            }
            catch (AssertionFailedException e)
            {
                exception = e;
            }
            Assert.IsNotNull(exception, "Expect exception to be thrown");
        }


        protected class CallChainAssert
        {
            private readonly IList<string> m_messages = new List<string>();

            internal void Called(string msg)
            {
                m_messages.Add(msg);
            }

            internal void AssertCalledInOrder(string expectCallChain)
            {
                var actual = string.Join(",", m_messages);
                Assert.AreEqual(expectCallChain, actual, "incorrect calls made, or missing calls");
            }

        }
    }
}
