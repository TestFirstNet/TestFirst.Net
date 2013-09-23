using System;

namespace TestFirst.Net.Examples.Service.Handler
{
    interface IHandler
    {
        Object Handle(Object query);
    }

    interface IHandler<TQuery, out TResponse> : IHandler
    {
        TResponse Handle(TQuery query);
    }
}
