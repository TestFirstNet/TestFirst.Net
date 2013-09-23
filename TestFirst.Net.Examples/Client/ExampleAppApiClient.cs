using System;
using TestFirst.Net.Examples.Api;
using TestFirst.Net.Examples.Api.Query;

namespace TestFirst.Net.Examples.Client
{
    public class ApiClient:IApiClient
    {
        public TResponse Query<TResponse>(IReturn<TResponse> query)
        {
            return (TResponse)Query(query,typeof(TResponse));
        }

        public TResponse Query<TQuery,TResponse>(TQuery query) where TResponse:class
        {
            return (TResponse)Query(query, typeof(TResponse));
        }

        private Object Query(Object query, Type responseType)
        {
            //serialize request

            //make call

            //deserialize response
            return null;
        }
    }
}
