using System;
using System.Net;

namespace TestFirst.Net.Examples.Service.Http
{
    internal class RequestContext
    {
        public Guid Id { get; private set; }
        public HttpListenerRequest HttpRequest { get; private set; }
        public HttpListenerResponse HttpResponse { get; private set; }
            
        internal RequestContext(Guid id, HttpListenerRequest request, HttpListenerResponse response)
        {
            Id = id;
            HttpRequest = request;
            HttpResponse = response;
        }            
    }
}