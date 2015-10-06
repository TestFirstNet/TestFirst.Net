namespace TestFirst.Net.Examples.Net
{
    public interface IApiClient
    {
        TResponse Invoke<TResponse>(IReturn<TResponse> request);

        TResponse Invoke<TRequest, TResponse>(TRequest request) where TResponse : class;
    }
}
