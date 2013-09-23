using System;
using System.Threading;

namespace TestFirst.Net.Performance
{
    /// <summary>
    /// Adds in the test id before passing on to the test run listener
    /// </summary>
    internal class TestListenerAdapter : IPerformanceTestListener
    {
        private readonly IPerformanceTestRunnerListener m_listener;
        private readonly string m_agentId;
        private readonly string m_machineId;
        private TestId m_testId;

        public TestListenerAdapter(IPerformanceTestRunnerListener listener, PerformanceSuite.PerfTestContext ctxt)
        {
            m_listener = listener;
            m_agentId = ctxt.AgentId;
            m_machineId = ctxt.MachineId;
        }

        public void OnMetric(PerformanceMetric metric)
        {                
            m_listener.OnMetric(GetTestId(), metric);
        }

        public void OnError(Exception e)
        {
            m_listener.OnError(GetTestId(), e);
        }

        private TestId GetTestId()
        {
            if (m_testId == null)
            {
                m_testId = new TestId(
                    machineId:m_machineId,
                    agentId: m_agentId,
                    threadId: Thread.CurrentThread.ManagedThreadId.ToString()
                    );
            }
            return m_testId;
        }
    }
}