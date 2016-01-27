using System;
using System.Collections.Generic;
using TestFirst.Net.Examples.Api;
using TestFirst.Net.Examples.Api.Query;
using TestFirst.Net.Examples.Net;
using TestFirst.Net.Examples.Service.Handler;
using TestFirst.Net.Examples.Service.Inject;

namespace TestFirst.Net.Examples.Service
{
    public class ExampleApp : IApiClient
    {
        private readonly IDictionary<Type, HandlerHolder> m_holdersByRequestType = new Dictionary<Type, HandlerHolder>();

        private readonly DependencyInjector m_injector = new DependencyInjector();

        public TResponse Invoke<TResponse>(IReturn<TResponse> query)
        {
            return (TResponse)Invoke(query, typeof(TResponse));
        }

        public TResponse Invoke<TQuery, TResponse>(TQuery query) 
            where TResponse : class
        {
            return (TResponse)Invoke(query, typeof(TResponse));
        }

        internal void RegisterHandler<TRequest, TResponse>(IHandler<TRequest, TResponse> handler)
        {
            // simply always return the same instance
            RegisterHandler(() => handler);
        }

        internal void RegisterHandler<TRequest, TResponse>(Func<IHandler<TRequest, TResponse>> handlerFactory)
        {
            m_holdersByRequestType.Add(typeof(TRequest), HandlerHolder.Create(handlerFactory));
        }

        private object Invoke(object query, Type responseType) 
        {
            var key = query.GetType();
            HandlerHolder holder;
            if (!m_holdersByRequestType.TryGetValue(key, out holder))
            {
                throw new ArgumentException("No handlers registered for query type:" + key.FullName);
            }

            var handler = holder.CreateHandler();
            
            m_injector.Inject(handler);

            var response = handler.Handle(query);
            if (response != null && response.GetType() != responseType)
            {
                throw new InvalidOperationException("Expected response type " + responseType.FullName + " but got " + response.GetType().FullName);
            }
            return response;
        }

        private class HandlerHolder
        {
            private readonly Func<IHandler> m_handlerFactory;
            internal HandlerHolder(Func<IHandler> handlerFactory)
            {
                m_handlerFactory = handlerFactory;
            }

            internal static HandlerHolder Create<TRequest, TResponse>(Func<IHandler<TRequest, TResponse>> handlerFactory)
            {
                return new HandlerHolder(handlerFactory);
            }

            internal IHandler CreateHandler()
            {
                return m_handlerFactory.Invoke();
            }
        }
    }
}
