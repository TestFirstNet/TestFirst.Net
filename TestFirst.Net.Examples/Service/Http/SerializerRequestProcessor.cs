using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace TestFirst.Net.Examples.Service.Http
{
    class SerializerRequestProcessor : IRequestProcessor
    {
        private readonly IDictionary<string,ISerializer> m_contentTypeToSerializer;
        private const String DefaultContentType = "*";

        public static Builder With()
        {
            return new Builder();
        }

        private SerializerRequestProcessor(IDictionary<String,ISerializer> serializersByContentType)
        {
            m_contentTypeToSerializer = new Dictionary<string, ISerializer>(serializersByContentType);
        }

        public void Process(IRequestProvider provider)
        {
            var request = provider.Take();

            var httpRequest = request.HttpRequest;
            var httpResponse = request.HttpResponse;

            Console.WriteLine("Request:" + httpRequest.RawUrl);
            //todo:pre filters?
            if (!httpResponse.Completed())//maybe filter has finished or client has disconnected
            {
                var serializer = LookupSerializer(httpRequest);
                var requestDto = ReadRequestDto(httpRequest, serializer);
                var responseDto = GenerateResponseDto(httpRequest, httpResponse, requestDto);

                //maybe response has already been written to and closed, in which case we don't need to do anything
                if (!httpResponse.Completed())
                {
                    WriteResponseDto(httpResponse, serializer, responseDto);
                }

                //todo:post filters?
            }
        }

        private MyDummyResponse GenerateResponseDto(HttpListenerRequest httpRequest, HttpListenerResponse httpResponse, Object requestDto)
        {
            var path = ExtractPath(httpRequest);
            var method = httpRequest.HttpMethod;
            var responseDto = new MyDummyResponse
                {
                    Message = "Hello World",
                    Debug = "path:" + path + ", method:" + method + ",thread" + Thread.CurrentThread.ManagedThreadId
                };
            return responseDto;
        }

        private static void WriteResponseDto(HttpListenerResponse response, ISerializer serializer, MyDummyResponse responseDto)
        {
            using (var writer = new StreamWriter(response.OutputStream, Encoding.UTF8))
            {
                serializer.Serialize(writer, responseDto);
            }
        }

        private Object ReadRequestDto(HttpListenerRequest request, ISerializer serializer)
        {
            var convertTo = LookupTargetType(request);

            Object requestDto;
            using (var stream = request.InputStream)
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                requestDto = serializer.Deserialize(reader, convertTo);
            }
            return requestDto;
        }

        private ISerializer LookupSerializer(HttpListenerRequest request)
        {
            var contentType = request.ContentType ?? DefaultContentType;
            ISerializer serializer;
            if (!m_contentTypeToSerializer.TryGetValue(contentType, out serializer))
            {
                if (!m_contentTypeToSerializer.TryGetValue(DefaultContentType, out serializer))
                {
                    throw new ArgumentException("No serializer found for content type" + contentType);
                }
            }
            return serializer;
        }

        private Type LookupTargetType(HttpListenerRequest request)
        {
            return typeof (MyDummyRequest);
        }

        private String ExtractPath(HttpListenerRequest request)
        {
            var uri = request.Url;
            var pathQuery = uri.PathAndQuery;
            var question = pathQuery.IndexOf('?');
            if (question != -1)
            {
                return pathQuery.Substring(0,question);
            }
            return pathQuery;
        }

        class MyDummyRequest
        {

        }

        class MyDummyResponse
        {
            public String Message { get; set; }
            public String Debug { get; set; }

        }

        public class Builder
        {
            private IDictionary<String,ISerializer> m_contentTypeToSerializer = new Dictionary<string, ISerializer>();

            public Builder DefaultSerializer(ISerializer serializer)
            {
                AddSerializer(DefaultContentType,serializer);
                return this;
            }

            public Builder AddSerializer(String contentType, ISerializer serializer)
            {
                m_contentTypeToSerializer.Add(contentType,serializer);
                return this;
            }

            public SerializerRequestProcessor Build()
            {
                return new SerializerRequestProcessor(m_contentTypeToSerializer);
            }

        }

     
    }
}