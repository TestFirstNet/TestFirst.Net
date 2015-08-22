using System;

namespace TestFirst.Net.Inject
{
    public interface IClock
    {
        DateTimeOffset Now();
    }
}
