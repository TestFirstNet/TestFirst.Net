using System;
using TestFirst.Net.Examples.Api;
using TestFirst.Net.Examples.Api.Query;

namespace TestFirst.Net.Examples.Client
{
    public class ApiClient:IApiClient
    {
        public TResponse Invoke<TResponse>(IReturn<TResponse> query)
        {
            return (TResponse)Invoke(query,typeof(TResponse));
        }

        public TResponse Invoke<TQuery,TResponse>(TQuery query) where TResponse:class
        {
            return (TResponse)Invoke(query, typeof(TResponse));
        }

        private Object Invoke(Object query, Type responseType)
        {
            //serialize request

            //make call

            //deserialize response
            return null;
        }
    }
}
