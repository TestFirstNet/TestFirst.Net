using System;

namespace TestFirst.Net.Performance
{
    /// <summary>
    /// Logs test metrics to the console
    /// </summary>
    public class ConsolePerformanceTestRunnerListener : IPerformanceTestRunnerListener
    {
        public void OnBeginTestSession()
        {
            Console.WriteLine("OnBeginTestSession");
        }

        public void OnBeginTestRun()
        {
            Console.WriteLine("OnBeginTestRun");
        }

        public void OnMetric(TestId testId, PerformanceMetric metric)
        {
            Console.WriteLine(testId + ", " + metric);
        }

        public void OnError(TestId testId, Exception testException)
        {
            Console.WriteLine(testId + ", Exception:" + testException);
        }

        public void OnEndTestRun()
        {
            Console.WriteLine("OnEndTestRun");
        }

        public void OnEndTestSession()
        {
            Console.WriteLine("OnEndTestSession");
        }
    }
}