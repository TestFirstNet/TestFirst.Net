using System;

namespace TestFirst.Net.Performance
{
    /// <summary>
    /// Used to pass metrics back from the tests to be collected elsewhere and analysed
    /// </summary>
    public interface IPerformanceTestListener
    {
        void OnMetric(PerformanceMetric metric);

        void OnError(Exception e);
    }
}