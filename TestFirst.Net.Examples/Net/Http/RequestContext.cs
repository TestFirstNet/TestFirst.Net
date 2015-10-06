using System;
using System.Net;

namespace TestFirst.Net.Examples.Net.Http
{
    internal class RequestContext
    {
        internal RequestContext(Guid id, HttpListenerRequest request, HttpListenerResponse response)
        {
            Id = id;
            HttpRequest = request;
            HttpResponse = response;
        }

        public Guid Id { get; private set; }
        public HttpListenerRequest HttpRequest { get; private set; }
        public HttpListenerResponse HttpResponse { get; private set; }
    }
}