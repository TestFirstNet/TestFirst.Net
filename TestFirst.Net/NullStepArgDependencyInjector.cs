namespace TestFirst.Net
{
    /// <summary>
    /// Listener which does nothing
    /// </summary>
    public sealed class NullStepArgDependencyInjector : IStepArgDependencyInjector
    {
        public void InjectDependencies<T>(T instance)
        {
        }

        public void Dispose()
        {
        }
    }
}
