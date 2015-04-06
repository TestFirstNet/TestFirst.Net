using System;
using System.Net;
using System.Threading;
using NUnit.Framework;
using TestFirst.Net.Extensions.NUnit;

namespace TestFirst.Net.Examples.Service.Http
{   
    [TestFixture]
    public class ListenerTest : AbstractNUnitScenarioTest
    {
        [Test]
        public void Test()
        {
            var listener = SimpleServer.With()
                .Host("localhost")
                .FindFreePort()
                .AuthScheme(AuthenticationSchemes.Anonymous)
                .RequestProcessor(SerializerRequestProcessor.With()
                    .DefaultSerializer(new JsonNetSerializer()).Build()).Build();

            listener.StartNonBlocking();

            var host = listener.Host;
            var port = listener.Port;
            var url = "http://" + host + ":" + port;
            //TODO:make request
            
            Thread.Sleep(TimeSpan.FromSeconds(1));

            listener.Stop();
        }
    }
}
