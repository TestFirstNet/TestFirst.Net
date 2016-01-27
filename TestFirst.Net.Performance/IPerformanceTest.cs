namespace TestFirst.Net.Performance
{
    public interface IPerformanceTest
    {
        /// <summary>
        /// Perform the actual test using the provided lister to pass back metrics. 
        /// </summary>
        /// <param name="testListener">The test listener</param>
        void InvokeTest(IPerformanceTestListener testListener);
    }
}