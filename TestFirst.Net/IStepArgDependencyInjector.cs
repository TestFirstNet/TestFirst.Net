using System;
using TestFirst.Net.Inject;

namespace TestFirst.Net
{
    [Obsolete("Use ITestInjector")]
    public interface IStepArgDependencyInjector:ITestInjector,IDisposable
    {
    }
}
