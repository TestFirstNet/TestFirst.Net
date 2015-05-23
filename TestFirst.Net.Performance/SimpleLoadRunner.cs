using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TestFirst.Net.Lang;
using TestFirst.Net.Logging;
using TestFirst.Net.Util;

namespace TestFirst.Net.Performance
{
    public class SimpleLoadRunner : PerformanceSuite.ILoadRunner
    {
        private readonly int m_numThreads;
        private readonly TimeSpan m_delayBetweenTests;
        private readonly double m_testDelayVariance;
        private readonly PerformanceSuite.ITestProvider m_testProvider;
        private readonly TimeSpan m_runFor;
        private readonly bool m_failOnError;
        private readonly ThreadPriority m_threadPriority;
        private readonly TimeSpan m_threadStartDelay;
        private readonly double m_threadStartDelayVariance;
        

        private DateTime m_endAt;
        private List<ActionCompleteThread> m_threads = new List<ActionCompleteThread>();

        private SimpleLoadRunner(PerformanceSuite.ITestProvider testProvider, int numThreads, TimeSpan runFor, TimeSpan delayBetweenTests, bool failOnError, ThreadPriority threadPriority, TimeSpan threadStartDelay, double threadStartDelayVariance, double testDelayVariance)
        {
            m_testProvider = testProvider;
            m_numThreads = numThreads;
            m_delayBetweenTests = delayBetweenTests;
            m_failOnError = failOnError;
            m_threadPriority = threadPriority;
            m_threadStartDelay = threadStartDelay;
            m_threadStartDelayVariance = threadStartDelayVariance;
            m_testDelayVariance = testDelayVariance;
            m_runFor = runFor;
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
            m_endAt = DateTime.Now.Add(m_runFor);
            m_threads.Clear();
            m_threads = new List<ActionCompleteThread>();
            for (var i = 01; i < m_numThreads; i++)
            {
                var threadListener = new TestListenerAdapter(runListener, ctxt);
                var threadAction = NewInvokeTestAction(threadListener);
                m_threads.Add(new ActionCompleteThread(threadAction.Invoke).Where(t=>t.Priority = m_threadPriority));
            }

            m_threads.ForEach(t => t.Start());
            //wait for runners to complete
            while (DateTime.Now < m_endAt && m_threads.Any(t=>!t.Completed))
            {
                Thread.Sleep(1000);
            }
            m_threads.ForEach(t => t.Abort());
        }

        public void Abort()
        {
            m_threads.ForEach(t => t.Abort());
        }

        private Action NewInvokeTestAction(IPerformanceTestListener listener)
        {
            return () =>
            {
                if (m_threadStartDelay.TotalMilliseconds > 0 && m_threadStartDelayVariance > 0)
                {
                    var r = new System.Random();
                    //ensure some random variance in the start time of each thread
                    var delayStart = r.NextDouble() * (m_threadStartDelayVariance * m_threadStartDelay.TotalMilliseconds);
                    if (delayStart > 0)
                    {
                        Thread.Sleep((int) delayStart);
                    }
                }
                
                while(DateTime.Now < m_endAt)
                {
                    try
                    {
                        var testAction = m_testProvider.Next();
                        testAction.InvokeTest(listener);
                    }
                    catch (Exception e)
                    {
                        listener.OnError(e);
                        if(m_failOnError)
                        {
                            throw;
                        }                       
                    }
                    if (m_delayBetweenTests.TotalMilliseconds > 0)
                    {
                        if (m_testDelayVariance > 0)
                        {
                            var r = new System.Random();
                            //ensure some random variance in the delay between tests to smooth the load
                            var delay = r.NextDouble() * (m_testDelayVariance * m_delayBetweenTests.TotalMilliseconds);
                            if (delay > 0)
                            {
                                Thread.Sleep((int)delay);
                            }
                        }
                        else
                        {
                            Thread.Sleep(m_delayBetweenTests);
                        }
                    }
                }
            };
        }

        public class Builder : IBuilder<PerformanceSuite.ILoadRunner>
        {
            private int m_numThreads = 1;            
            private TimeSpan m_delayBetweenThreads = TimeSpan.FromSeconds(1);
            private PerformanceSuite.ITestProvider m_testProvider;
            private TimeSpan? m_runFor;
            private bool m_failOnError = true;
            private ThreadPriority m_threadPriority = ThreadPriority.Normal;
            private TimeSpan m_startDelay = TimeSpan.FromSeconds(0);
            private double m_startDelayVariance = 0;
            private double m_testDelayVariance = 0;

            public PerformanceSuite.ILoadRunner Build()
            {
                PreConditions.AssertNotNull(m_testProvider, "TestProvider");
                PreConditions.AssertTrue(m_numThreads > 0, "num threads must be > 0");
                PreConditions.AssertNotNull(m_runFor, "expect time to run for");
                PreConditions.AssertTrue(m_startDelayVariance >= 0 && m_startDelayVariance <= 1, "expect thread start delay variance to be between 0 and 1");
                PreConditions.AssertTrue(m_testDelayVariance >= 0 && m_testDelayVariance <= 1, "expect test delay variance to be between 0 and 1");

                return new SimpleLoadRunner(
                    numThreads:m_numThreads,
                    delayBetweenTests:m_delayBetweenThreads, 
                    testDelayVariance:m_testDelayVariance,
                    runFor:m_runFor.GetValueOrDefault(), 
                    threadPriority:m_threadPriority, 
                    threadStartDelay:m_startDelay,
                    threadStartDelayVariance:m_startDelayVariance,
                    failOnError:m_failOnError,
                    testProvider: m_testProvider);
            }

            public Builder NumThreads(int numThreads)
            {
                PreConditions.AssertTrue(numThreads > 0, "Num threads must be > 0 but was " + numThreads);
                m_numThreads = numThreads;
                return this;
            }

            public Builder RunFor(TimeSpan time)
            {
                m_runFor = time;
                return this;
            }

            public TimeSpanBuilder<Builder> StartDelay(double delay)
            {
                return new TimeSpanBuilder<Builder>(delay, StartDelay);
            }

            public Builder StartDelay(TimeSpan time)
            {
                m_startDelay = time;
                return this;
            }

            public Builder StartDelayVariance(double variance)
            {
                m_startDelayVariance = variance;
                return this;
            }

            public Builder Priority(ThreadPriority priority)
            {
                m_threadPriority = priority;
                return this;
            }

            public Builder FailOnError(bool b)
            {
                m_failOnError = b;
                return this;
            }

            public Builder Tests(params IPerformanceTest[] tests)
            {
                Tests(RoundRobbin.With().Tests(tests));
                return this;
            }

            public Builder Tests(IBuilder<PerformanceSuite.ITestProvider> testProviderBuilder)
            {
                Tests(testProviderBuilder.Build());
                return this;
            }
            
            public Builder Tests(PerformanceSuite.ITestProvider testProvider)
            {
                m_testProvider = testProvider;
                return this;
            }

            public TimeSpanBuilder<Builder> DelayBetweenTests(double delay)
            {
                return new TimeSpanBuilder<Builder>(delay, DelayBetweenTests);
            }

            public Builder DelayBetweenTests(TimeSpan delay)
            {
                m_delayBetweenThreads = delay;
                return this;
            }

            public Builder DelayBetweenTestsVariance(double variance)
            {
                m_testDelayVariance = variance;
                return this;
            }

            public TimeSpanBuilder<Builder> RunFor(double time)
            {
                return new TimeSpanBuilder<Builder>(time, RunFor);
            }

            
        }
    }
}