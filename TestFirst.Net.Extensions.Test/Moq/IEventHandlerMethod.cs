using System;

namespace TestFirst.Net.Extensions.Test.Moq
{
    public interface IEventHandlerMethod<T>
        where T : EventArgs
    {
        void Fires(object sender, T args);
    }
}