using System.ComponentModel;

namespace TestFirst.Net.Extensions.Test.Moq
{
    public interface IPropertyChangedMethod
    {
        void Fires(object sender, PropertyChangedEventArgs args);
    }
}