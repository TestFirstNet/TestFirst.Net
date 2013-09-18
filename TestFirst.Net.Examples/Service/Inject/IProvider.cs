namespace TestFirst.Net.Examples.Service.Inject
{
    internal interface IProvider<T>
    {
        T Provide();
    }
}
