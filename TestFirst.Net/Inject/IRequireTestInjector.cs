using System;

namespace TestFirst.Net.Inject
{
    public interface IRequireTestInjector
    {
        ITestInjector TestInjector { set; }
    }
}

