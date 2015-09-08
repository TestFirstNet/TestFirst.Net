using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TestFirst.Net.Lang;
using TestFirst.Net.Logging;
using TestFirst.Net.Util;

namespace TestFirst.Net.Performance
{
    /// <summary>
    /// Runs the actual tests and informs the listeners. A 'Run' is essentially a single 'run' of all the tests. Each run is assigned a number of threads which actually runs each test.
    /// 
    /// So 3 runs of 5 thread means that 5 threads will be created and a test assigned to each thread and run. Then this is repeasted 2 more times.
    /// </summary>
    public class PerformanceSuite : IInvokable
    {
        private readonly ILogger Log = Logger.GetLogger<PerformanceSuite>();

        private readonly TimeSpan? m_runTimeout;
        private readonly IRunStrategy m_runStrategy;
        //called back with interesting test events. Used to collect metrics
        private readonly IPerformanceTestRunnerListener m_listener;
        private readonly  IList<ILoadRunner> m_loadRunners;

        public static Builder With()
        {
            return new Builder();
        }

        private PerformanceSuite(IEnumerable<ILoadRunner> testRunners, IRunStrategy runStrategy, IPerformanceTestRunnerListener listener,TimeSpan? runTimeout)
        {
            m_runStrategy = runStrategy;
            m_listener = listener;
            m_loadRunners = new List<ILoadRunner>(testRunners);
            m_runTimeout = runTimeout;
        }

        public void Invoke()
        {
            PreConditions.AssertTrue(m_loadRunners.Count > 0, "TestRunners", "greater than 0");
            PreConditions.AssertTrue(m_runTimeout.HasValue, "PerRunTimeout", "not null");
            PreConditions.AssertNotNull(m_listener, "Listener");    

            try
            {
                Log.Info("OnBeginTestSession");
                m_listener.OnBeginTestSession();
                BeforeInvoke(); 
                
                int runNum = 1;
                while (m_runStrategy.ShouldRun(runNum))
                {
                    Log.Info("OnBeginTestRun " + runNum);
                    runNum++;

                    m_listener.OnBeginTestRun();
                    try
                    {
                        var endBy = DateTime.Now.Add(m_runTimeout.Value);
                        var threads = new List<ActionCompleteThread>();
                        var ctxt = new PerfTestContext {AgentId = "0", MachineId = Environment.MachineName};
                        foreach (var runner in m_loadRunners)
                        {
                            var loader = runner;
                            var loaderAction = new ActionCompleteThread(() => loader.Start(ctxt, m_listener));
                            threads.Add(loaderAction);
                        }
                        threads.ForEach(t => t.Start());
                        //wait for runners to complete
                        while (DateTime.Now < endBy && threads.Any(t=>!t.Completed))
                        {
                            Thread.Sleep(1000);
                        }
                        
                        threads.ForEach(t => t.Abort());
                        //generate max contention
                        //ParallelActionInvoker.InvokeAllWaitingForCompletion(actions, m_runTimeout.Value);
                        Log.Info("OnEndTestRun");
                    }
                    finally
                    {
                        m_listener.OnEndTestRun();
                    }
                }                        
            }
            finally
            {
                Log.Info("OnEndTestSession");
                AfterInvoke();
                m_listener.OnEndTestSession();              
            }
        }

        private void BeforeInvoke()
        {
            Log.Debug("running BeforeInvoke");   
            foreach (var runner in m_loadRunners)
            {
                runner.BeforeInvoke();
            }
        }

        private void AfterInvoke()
        {
            Log.Debug("running AfterInvoke");  
            foreach (var runner in m_loadRunners)
            {
                 try
                {
                    runner.AfterInvoke();
                }
                catch (Exception e) 
                {
                    Log.Error("Error AfterInvoke for " + runner,e);
                }
            }
        }

        public class Builder : IBuilder<PerformanceSuite>
        {
            private readonly IList<ILoadRunner> m_testRunners = new List<ILoadRunner>();
            private IPerformanceTestRunnerListener m_listener = new ConsolePerformanceTestRunnerListener();
            private IRunStrategy m_runStrategy = RunStrategies.Fixed(1);
            private TimeSpan? m_runTimeout;

            public PerformanceSuite Build()
            {
                return new PerformanceSuite(m_testRunners,m_runStrategy,m_listener, m_runTimeout);
            }

            /// <summary>
            /// Just the given number of runs
            /// </summary>
            public Builder NumRuns(int numRuns)
            {
                NumRuns(RunStrategies.Fixed(numRuns));
                return this;
            }

            /// <summary>
            /// As many runs until the given time is up
            /// </summary>
            public Builder NumRuns(TimeSpan time)
            {
                NumRuns(RunStrategies.Time(time));
                return this;
            }

            public Builder NumRuns(IRunStrategy strategy)
            {
                PreConditions.AssertNotNull(strategy, "RunStrategy");
                m_runStrategy = strategy;
                return this;
            }

            public Builder Listener(IPerformanceTestRunnerListener listener)
            {
                m_listener = listener;
                return this;
            }
            
            public TimeSpanBuilder<Builder> PerRunTimeout(double time)
            {
                return new TimeSpanBuilder<Builder>(time, PerRunTimeout);
            }

            public Builder PerRunTimeout(TimeSpan timeout)
            {
                m_runTimeout = timeout;
                return this;
            }

            public Builder LoadRunner(IBuilder<ILoadRunner> runnerBuilder)
            {
                LoadRunner(runnerBuilder.Build());
                return this;
            }
        
            public Builder LoadRunner(ILoadRunner runner)
            {
                m_testRunners.Add(runner);
                return this;
            }
        }

        public class PerfTestContext
        {
            public String AgentId { get; internal set; }
            public String MachineId { get; internal set; }
        }

        //see http://www.soapui.org/Load-Testing/simulating-different-types-of-load.html for ideas
        public interface ILoadRunner : IAbortable
        {
            void BeforeInvoke();
            void Start(PerfTestContext context, IPerformanceTestRunnerListener listener);
            void AfterInvoke();
        }

        public interface IAbortable
        {
            void Abort();
        }

        public interface ITestProvider
        {
            IPerformanceTest Next();
        }
    }
}