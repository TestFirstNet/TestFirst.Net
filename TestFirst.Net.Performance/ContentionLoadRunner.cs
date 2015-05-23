using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TestFirst.Net.Concurrent;
using TestFirst.Net.Lang;
using TestFirst.Net.Logging;
using TestFirst.Net.Util;

namespace TestFirst.Net.Performance
{
    /// <summary>
    /// Runs a set of tests/actions in such a away to force maximuim thread contention
    /// </summary>
    public class ContentionLoadRunner : PerformanceSuite.ILoadRunner
    {
        private readonly IList<IPerformanceTest> m_tests;
        private readonly TimeSpan m_runTimeout;
        private readonly ThreadPriority m_threadPriority;
        private readonly static TimeSpan BarrierWaitTimeout = TimeSpan.FromSeconds(40);

        private ContentionLoadRunner(IEnumerable<IPerformanceTest> tests, TimeSpan runTimeout, ThreadPriority threadPriority)
        {
            m_threadPriority = threadPriority;
            m_runTimeout = runTimeout;
            m_tests = new List<IPerformanceTest>(tests);
        }

        public static Builder With()
        {
            return new Builder();
        }

        public void BeforeInvoke()
        {
            //throw new NotImplementedException();
        }

        public void AfterInvoke()
        {
            //throw new NotImplementedException();
        }

        public void Start(PerformanceSuite.PerfTestContext ctxt, IPerformanceTestRunnerListener runListener)
        {
            var actions = TestsToParallelActions(m_tests, runListener);
            ParallelActionInvoker.InvokeAllWaitingForCompletion(actions, m_runTimeout, m_threadPriority);
        }

        public void Abort()
        {
        }

        private IEnumerable<Action> TestsToParallelActions(IList<IPerformanceTest> tests,IPerformanceTestRunnerListener listener)
        {
            //barrier to synchronize Before/After operations
            var testSynchronizationBarrier = new Barrier(tests.Count);
            var actions = tests.Select(test => TestToAction(test, testSynchronizationBarrier, listener)).ToList();
            return actions;
        }


        private Action TestToAction(IPerformanceTest test, Barrier barrier, IPerformanceTestRunnerListener listener)
        {
            return () =>
                {
                    if (!barrier.SignalAndWait(BarrierWaitTimeout))
                    {
                        throw new TestFirstException("Timed out waiting for 'Invoke' barrier");
                    }
                    var testListener = new TestListenerAdapter(listener, new PerformanceSuite.PerfTestContext {AgentId = "0", MachineId = Environment.MachineName } );
                    test.InvokeTest(testListener);

                    if (!barrier.SignalAndWait(BarrierWaitTimeout))
                    {
                        throw new TestFirstException("Timed out waiting for 'AfterInvokeInTestThread' barrier");
                    }                    
                };
        }


        public class Builder : IBuilder<ContentionLoadRunner>
        {
            private IList<IPerformanceTest> m_tests;
            private TimeSpan? m_runTimeout;
            private ThreadPriority m_threadPriority = ThreadPriority.Normal;
            private int m_maxNumTests = 1000;

            public ContentionLoadRunner Build()
            {
                PreConditions.AssertNotNull(m_tests, "Tests");
                PreConditions.AssertNotNull(m_runTimeout, "expect run timeout");

                return new ContentionLoadRunner(m_tests,runTimeout:m_runTimeout.GetValueOrDefault(), threadPriority:m_threadPriority);
            }

            public Builder Priority(ThreadPriority priority)
            {
                m_threadPriority = priority;
                return this;
            }

            public Builder Tests(params IPerformanceTest[] tests)
            {
                 m_tests = tests.ToList();
                return this;
            }

            public Builder Tests(IBuilder<PerformanceSuite.ITestProvider> testProviderBuilder)
            {
                Tests(testProviderBuilder.Build());
                return this;
            }
            
            public Builder Tests(PerformanceSuite.ITestProvider testProvider)
            {
                var tests = new List<IPerformanceTest>();
                IPerformanceTest test;
                while( (test = testProvider.Next()) != null && tests.Count < m_maxNumTests)
                {
                    tests.Add(test);
                }
                m_tests = tests;
                return this;
            }

            /// <summary>
            /// If using a <see cref="PerformanceSuite.ITestProvider"/> the max number of tests to grab
            /// </summary>
            /// <param name="numTests"></param>
            /// <returns></returns>
            public Builder MaxNumTests(int numTests)
            {
                m_maxNumTests = numTests;
                return this;
            }

            public TimeSpanBuilder<Builder> RunTimeout(double time)
            {
                return new TimeSpanBuilder<Builder>(time, RunTimeout);
            }

            public Builder RunTimeout(TimeSpan time)
            {
                m_runTimeout = time;
                return this;
            }
        }
    }
}