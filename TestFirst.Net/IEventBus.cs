using System;

namespace TestFirst.Net
{
    public interface IEventBus
    {
        void FireEvent(object src,TestEvent evt);
    }
}

