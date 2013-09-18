namespace TestFirst.Net.Performance
{
    public interface IRunStrategy
    {
        /// <summary>
        /// Called to determine if there should be another run
        /// </summary>
        /// <param name="runNum">the 1 based number of the run</param>
        bool ShouldRun(int runNum);
    }
}