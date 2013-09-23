namespace TestFirst.Net
{
    /// <summary>
    /// Mark an instance to be run on scenario completion
    /// </summary>
    public interface IRunOnScenarioEnd
    {
        /// <summary>
        /// Invoked when the scenario ends. An exception causes the scenario to fail
        /// </summary>
        void OnScenarioEnd();
    }
}