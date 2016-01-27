using TestFirst.Net.Inject;

namespace TestFirst.Net
{
    /// <summary>
    /// Performs all the default injection. Also disposes of disposables
    /// </summary>
    public class SimpleStepArgInjector : DisposingStepArgInjector
    {
        public SimpleStepArgInjector()
        {
            Clock = new SystemClock();
        }

        public IClock Clock { get; set; }

        public override void InjectDependencies<T>(T instance)
        {
            base.InjectDependencies(instance);

            if (instance is IRequireClock) 
            {
                ((IRequireClock)instance).Clock = Clock;
            }
        }
    }
}
