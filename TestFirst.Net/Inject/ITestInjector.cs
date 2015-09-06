using System;

namespace TestFirst.Net.Inject
{
    public interface ITestInjector
    {
        void InjectDependencies<T>(T instance);
    }
}

