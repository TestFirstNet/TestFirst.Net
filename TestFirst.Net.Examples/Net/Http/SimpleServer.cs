using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace TestFirst.Net.Examples.Service.Http
{
    /// <summary>
    /// Simple http listener which listens for requests, puts them ina  queue for processing and then executes 
    /// a bunch of worker threads to process the requests
    /// 
    /// TODO:make worker queue external? why should we control threading?
    /// </summary>
    internal class SimpleServer : IDisposable
    {
        public String Host { get; private set; }
        public int Port { get; private set; }
        private readonly AuthenticationSchemes m_authScheme;
        private readonly AuthenticationSchemeSelector m_authSchemeSelector;
        private readonly IRequestProcessor m_requestProcessor;

        private volatile bool m_listen = true;
        private HttpListener m_listener;
        private Thread m_listeningThread;


        private readonly Object m_lock = new Object();

        public static Builder With()
        {
            return new Builder();
        }

        private SimpleServer(String host, int port, AuthenticationSchemes scheme, AuthenticationSchemeSelector authSchemeSelector, IRequestProcessor requestProcessor)
        {
            Host = host;
            Port = port;
            m_authScheme = scheme;
            m_authSchemeSelector = authSchemeSelector;
            m_requestProcessor = requestProcessor;
         
            if (Port <= 0)
            {
                throw new ArgumentException("Need a port higher than 0");
            }
            if (m_requestProcessor == null)
            {
                throw new ArgumentException("No work prcessor configured");
            }
        }

        /// <summary>
        /// Start the listener and block on the current thread
        /// </summary>
        public void StartAndWait()
        {
            StartNonBlocking();
            m_listeningThread.Join();
        }

        //TODO:OnException(Exception e){}

        /// <summary>
        /// Start the listener and return once started. Do no block the current thread
        /// </summary>
        public void StartNonBlocking()
        {
            lock (m_lock)
            {
                Stop();

                Log("starting");

                m_listener = new HttpListener();
                m_listener.Prefixes.Add("http://" + Host + ":" + Port + "/");
                m_listener.AuthenticationSchemes = m_authScheme;
                if( m_authSchemeSelector != null )
                {
                    m_listener.AuthenticationSchemeSelectorDelegate = m_authSchemeSelector;
                }
                m_listener.Start();
                m_listen = true;

                //TODO:how many listening threads?
                m_listeningThread = new Thread(() => 
                { 
                    while (m_listen && m_listener.IsListening)
                    {
                        var ctxt = m_listener.BeginGetContext(new AsyncCallback(PushToWorker),m_listener);
                        //sleep until a request comes in
                        try
                        {
                            ctxt.AsyncWaitHandle.WaitOne();
                        }
                        catch (ThreadInterruptedException)
                        {
                            //done listening, do nothing
                        }
                    }
                });

                //TODO: force abort on shutdown? need to register it?
                m_listeningThread.Start();
                
                Log("started listening on {0} {1}",Host,Port);
            }
        }

        public void Stop()
        {
            lock (m_lock)
            {
                if (m_listener != null && m_listener.IsListening)
                {
                    Log("Stopping");
                    m_listener.Stop();
                    m_listen = false;

                    if(m_listeningThread != null)
                    {
                        m_listeningThread.Interrupt();
                        m_listeningThread.Join();
                    }
                    Log("Stopped");
                }
            }
        }

        /// <summary>
        /// Stop the service and dispose of everything. You can't call Start again
        /// </summary>
        public void Dispose()
        {
            lock (m_lock)
            {
                Stop();

                if (m_listener != null)
                {
                    m_listen = false;
                    //wait for a bit to finish any work requests?
                    m_listener.Close();
                }
            }
        }

        private void PushToWorker(IAsyncResult callback)
        {
            if (m_listener.IsListening)
            {
                var provider = new RequestProvider(this, callback);
                m_requestProcessor.Process(provider);
            }
        }

        private RequestContext GetRequestFor(IAsyncResult result)
        {
            if (m_listener.IsListening)
            {
                var ctxt = m_listener.EndGetContext(result);
                return new RequestContext(Guid.NewGuid(), ctxt.Request, ctxt.Response);
            }
            //TODO:should allow all existing requests to finish no?
            throw new InvalidOperationException("Listener is no longer listening, can't provide any more work");
        }

        private class RequestProvider : IRequestProvider
        {
            private readonly IAsyncResult m_result;
            private readonly SimpleServer m_listener;
            private RequestContext m_request;

            public RequestProvider(SimpleServer listener, IAsyncResult result)
            {
                m_result = result;
                m_listener = listener;
            }

            public RequestContext Take()
            {
                //TODO:any memory barriers around here?
                if(m_request == null)
                {
                    m_request = m_listener.GetRequestFor(m_result);
                }
                return m_request;
            }
        }

        private void Log(String msg, params Object[] args)
        {
            Console.WriteLine(String.Format("DEBUG [HttpListener] " + msg, args));
        }

        public class Builder
        {
            private String m_host = "+";
            private int m_port = -1;
            private IRequestProcessor m_requestProcessor;
            private AuthenticationSchemes m_authScheme = AuthenticationSchemes.Anonymous;
            private AuthenticationSchemeSelector m_authSchemeSelector;

            public Builder Host(String host)
            {
                m_host = host;
                return this;
            }

            public Builder FindFreePort()
            {
                Port(-1);
                return this;
            }

            public Builder Port(int port)
            {
                m_port = port;
                return this;
            }

            public Builder AuthScheme(AuthenticationSchemes scheme)
            {
                m_authScheme = scheme;
                return this;
            }

            public Builder AuthSchemeSelector(AuthenticationSchemeSelector selector)
            {
                m_authSchemeSelector = selector;
                return this;
            }

            public Builder RequestProcessor(IRequestProcessor processor)
            {
                m_requestProcessor = processor;
                return this;
            }

            public SimpleServer Build()
            {
                var port = m_port;
                if (port <= 0)
                {
                    port = FindFreePortOn(m_host);
                }
                return new SimpleServer(m_host, port, m_authScheme, m_authSchemeSelector, m_requestProcessor);
            }

            private int FindFreePortOn(string host)
            {
                TcpListener listener = new TcpListener(Dns.GetHostAddresses(host)[0], 0);
                listener.Start();
                int port = ((IPEndPoint)listener.LocalEndpoint).Port;
                listener.Stop();
                return port;
            }
        }
    }
}
