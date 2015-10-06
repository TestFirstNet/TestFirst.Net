using System;
using System.Collections.Generic;
using Castle.DynamicProxy;

namespace TestFirst.Net.Extensions.Moq
{
    internal class EventInterceptor : IInterceptor
    {
        private readonly Dictionary<string, DelegateWrapper> m_handlers = new Dictionary<string, DelegateWrapper>();

        public void Intercept(IInvocation invocation)
        {
            if (invocation.Method.Name.StartsWith("add_") && invocation.Method.IsSpecialName)
            {
                var eventName = invocation.Method.Name.Substring("add_".Length);
                DelegateWrapper handler;
                if (!m_handlers.TryGetValue(eventName, out handler))
                {
                    handler = new DelegateWrapper();
                    m_handlers[eventName] = handler;
                }
                var next = RaisedEvent.Next;
                RaisedEvent.Next = null;
                if (next != null)
                    handler.Invoke(next.Sender, next.Args);  // raise the event
                else
                    handler.Add((Delegate)invocation.Arguments[0]);
                invocation.ReturnValue = handler.Delegate;
            }
            else if (invocation.Method.Name.StartsWith("remove_") && invocation.Method.IsSpecialName)
            {
                var eventName = invocation.Method.Name.Substring("remove_".Length);
                DelegateWrapper handler;
                if (m_handlers.TryGetValue(eventName, out handler))
                {
                    handler.Remove((Delegate)invocation.Arguments[0]);
                    invocation.ReturnValue = handler.Delegate;
                }
                invocation.ReturnValue = null;
            }
            else invocation.Proceed();
        }
    }
}
