namespace TestFirst.Net.Performance
{
    /// <summary>
    /// Subclass this and override just those methods you need. No need to call this from subclasses
    /// </summary>
    public abstract class BasePerformanceTest : IPerformanceTest
    {
        public abstract void InvokeTest(IPerformanceTestListener testListener);
    }
}