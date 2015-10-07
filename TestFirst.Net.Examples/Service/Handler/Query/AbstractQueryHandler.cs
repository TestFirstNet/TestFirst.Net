using System;

namespace TestFirst.Net.Examples.Service.Handler.Query
{
    internal abstract class AbstractQueryHandler<TQuery, TResponse> : IHandler<TQuery, TResponse>
    {        
        public object Handle(object query)
        {
            if (query == null)
            {
                throw new ArgumentException("Expected non null query");
            }
            
            if (query.GetType() != typeof(TQuery))
            {
                throw new ArgumentException("Expected query of type " + typeof(TQuery).FullName + " but got " + query.GetType().FullName);
            }
            return Handle((TQuery)query);
        }

        public abstract TResponse Handle(TQuery query);
    }
}
