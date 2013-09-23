using TestFirst.Net.Examples.Api.Query;

namespace TestFirst.Net.Examples.Api
{
    public interface IApiClient
    {
        TResponse Query<TResponse>(IReturn<TResponse> query);

        TResponse Query<TQuery, TResponse>(TQuery query) where TResponse : class;
    }
}
