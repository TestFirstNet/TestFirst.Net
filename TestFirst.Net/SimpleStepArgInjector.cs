using System;
using System.Collections.Generic;
using TestFirst.Net.Inject;

namespace TestFirst.Net
{
    /// <summary>
    /// Performs all the default injection. Also disposes of disposables
    /// </summary>
    public class SimpleStepArgInjector : DisposingStepArgInjector
    {
        public IClock Clock {get;set;}

        public SimpleStepArgInjector(){
            Clock = new Inject.SystemClock();
        }

        override public void InjectDependencies<T>(T instance)
        {
            base.InjectDependencies(instance);

            if (instance is IRequireClock) {
                ((IRequireClock)instance).Clock = Clock;
            }
        }


    }
}
