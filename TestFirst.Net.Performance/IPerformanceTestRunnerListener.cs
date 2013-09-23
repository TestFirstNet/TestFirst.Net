using System;

namespace TestFirst.Net.Performance
{
    /// <summary>
    /// Listens to the entire test run
    /// </summary>
    public interface IPerformanceTestRunnerListener
    {
        void OnBeginTestSession();
        void OnBeginTestRun();
        void OnMetric(TestId testId, PerformanceMetric metric);
        void OnError(TestId testId, Exception testException);
        void OnEndTestRun();
        void OnEndTestSession();
    }
}
