using System;
using TestFirst.Net;

namespace TestFirst.Net.Inject
{
    public interface IRequireEventBus
    {
        IEventBus TestEventBus { set; }
    }
}

