using TestFirst.Net.Examples.Api.Query;

namespace TestFirst.Net.Examples.Api
{
    public interface IApiClient
    {
        TResponse Invoke<TResponse>(IReturn<TResponse> request);

        TResponse Invoke<TRequest, TResponse>(TRequest request) where TResponse : class;
    }
}
