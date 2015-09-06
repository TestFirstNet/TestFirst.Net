using System;
using System.Collections.Generic;
using TestFirst.Net.Inject;
using Inject;
using System.Threading.Tasks;

namespace TestFirst.Net.Inject
{
    /// <summary>
    /// Performs all the default injection. Also disposes of disposables. Ensures objects
    /// are injected only once
    /// </summary>
    public class TestInjector : AbstractTestInjector
    {
        public IClock Clock {get;set;}
        public IEventBus TestEventBus {get;set;}

        public TestInjector(){
            Clock = new Inject.SystemClock();
            TestEventBus = new EventBus ();

            AddGuard (new InjectOnlyOnceGuard());
            AddChildInjector (new DisposingInjector());
        }

        protected override void OnInject(object instance){
            if (instance is IRequireClock) {
                ((IRequireClock)instance).Clock = Clock;
            }
            if (instance is IRequireEventBus) {
                ((IRequireEventBus)instance).TestEventBus = TestEventBus;
            }

            if (instance is IRequireTestInjector) {
                ((IRequireTestInjector)instance).TestInjector = this;
            }
        }

        protected override void AfterInject (object instance)
        {
            var postInject = instance as IRequirePostInjectNotification;
            if (postInject != null) {
                postInject.OnAfterInject ();
            }
        }

    }
}
