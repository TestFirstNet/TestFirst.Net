using System;

namespace TestFirst.Net
{
    public abstract class TestEvent : EventArgs
    {
        public object Target { get; }

        protected TestEvent(object target)
        {
            Target = target;
        }

        public delegate void Handler(object src,TestEvent args);

    }
}

