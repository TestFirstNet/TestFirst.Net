using TestFirst.Net.Inject;

namespace TestFirst.Net
{
    /// <summary>
    /// Listener which does nothing
    /// </summary>
    public sealed class NullStepArgDependencyInjector : ITestInjector
    {
        public void InjectDependencies<T>(T instance)
        {
        }

        public void Dispose()
        {
        }
    }
}
