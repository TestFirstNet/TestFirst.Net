using System;
using System.ComponentModel;

namespace TestFirst.Net.Extensions.Test.Moq
{
    public interface ITestInterface : INotifyPropertyChanged
    {
        event EventHandler<TestEventArgs> SomeEvent;
    }
}