using System;

namespace TestFirst.Net.Examples.Service.Handler
{
    internal interface IHandler
    {
        object Handle(object query);
    }

    internal interface IHandler<in TQuery, out TResponse> : IHandler
    {
        TResponse Handle(TQuery query);
    }
}
