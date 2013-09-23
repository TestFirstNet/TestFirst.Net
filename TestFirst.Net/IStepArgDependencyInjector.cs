using System;

namespace TestFirst.Net
{
    public interface IStepArgDependencyInjector:IDisposable
    {
        void InjectDependencies<T>(T instance);
    }
}
