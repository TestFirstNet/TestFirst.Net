using System;
using System.Net;
using System.Threading;
using System.Linq;
using System.Text;
using System.Configuration;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TestFirst.Net.Web
{
    public delegate void Handler(HttpListenerContext ctxt);

    public class AppServer : IDisposable,IInvokable
    {
        private readonly HttpListener _listener = new HttpListener();
        private readonly Handler _handler;
        private readonly IWorkQueue _workQueue;

        public readonly string BaseUrl;

        public static Builder With(){
            return new Builder ();
        }

        public AppServer (string[] prefixes,Handler handler, IWorkQueue workQueue)
        {
         
            if (!HttpListener.IsSupported)
                throw new NotSupportedException("Http listener not supported");


            if (handler == null)
                throw new ArgumentNullException("expect request handler");

            if (workQueue == null)
                throw new ArgumentNullException("expect work queue");

            string baseUrl = null;
            foreach (string s in prefixes) {
                if (baseUrl == null) {
                    baseUrl = s;
                }
                _listener.Prefixes.Add (s);
            }


            BaseUrl = baseUrl;
            _workQueue = workQueue;
            _handler = handler;
            _listener.Start();

        }

        public void Invoke(){
            Start();
        }

        public void Dispose(){
            Stop();
        }

        public void Start()
        {
            Console.WriteLine("Starting web server");

            _workQueue.Push(() =>
                {
                    Console.WriteLine("Webserver running...");
                    try
                    {
                        while (_listener.IsListening)
                        {
                            _workQueue.Push((c) =>
                                {
                                    var ctxt = c as HttpListenerContext;
                                    try {
                                        _handler(ctxt);
                                    } catch (Exception e){ 
                                        Console.WriteLine("ctxt error:" + e.ToString());
                                    }
                           /*         try
                                    {
                                        string rstr = _responderMethod(ctx.Request);
                                        byte[] buf = Encoding.UTF8.GetBytes(rstr);
                                        ctx.Response.ContentLength64 = buf.Length;
                                        ctx.Response.OutputStream.Write(buf, 0, buf.Length);
                                    }
                                    catch (Exception e){ 
                                        Console.WriteLine("ctxt error:" + e.ToString());
                                    }
                                    finally
                                    {
                                        // always close the stream
                                        ctx.Response.OutputStream.Close();
                                    }
                                    */
                                }, (HttpListenerContext)_listener.GetContext());
                        }
                        Console.WriteLine("Webserver end listening");

                    } catch (Exception e){
                        Console.WriteLine("Http listener error:" + e.ToString());
                    }

                });
        }


        public void Stop()
        {
            _listener.Stop();
            _listener.Close();
        }



        public class Builder : IBuilder<AppServer> {

            private int port = -1;
            private string host = null;
            private IList<string> prefixes = new List<string>();
            private IDictionary<Regex,Handler> handlers = new Dictionary<Regex,Handler>();

            public AppServer Build(){
                return new AppServer (GetPrefixesOrDefault(),GetHandlerOrDefault(),GetWorkQueueOrDefault());
            }

            public Builder Defaults(){
                return this;
            }

            private IWorkQueue GetWorkQueueOrDefault(){
                return new ThreadWorkerQueue();
            }

            private Handler GetHandlerOrDefault(){
                return new MatchingHandler (handlers).Handle;
            }


            private string[] GetPrefixesOrDefault(){
                if(this.prefixes.Count == 0){
                    return new string[]{ GetDefaultPrefix() };  
                }
                return this.prefixes.ToArray ();
            }

            private string GetDefaultPrefix(){
                var host = this.host == null ? "127.0.0.1" : this.host;
                var port = this.port <= 0 ? FindFreePort (host) : this.port;
                return "http://" + host + ":" + port + "/";
            }

            private int FindFreePort(string host){

                return 9090;
            }


            public Builder Host(string hostname){
                this.host = hostname;
                return this;
            }

            public Builder Port(int port){
                this.port = port;
                return this;
            }

            public Builder Handler(string pathExpression, Func<HttpListenerContext,object> func){
                Handler(pathExpression, new SimpleHandler(func).Handle);
                return this;
            }

            public Builder Handler(string pathExpression, Handler handler){
                Handler(ExpressionToReqExp (pathExpression), handler);
                return this;
            }

            public Builder Handler(Regex rePathExpression, Handler handler){
                this.handlers.Add (rePathExpression, handler);
                return this;
            }

            private static Regex ExpressionToReqExp(string exp){
                var pattern = exp
                    .Replace (".", "\\.")
                    .Replace ("?", ".?")
                    .Replace ("**", "^^^")
                    .Replace ("*", "[^/]*")
                    .Replace ("^^^", ".*");
                return new Regex (pattern, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
            }


            public Builder Prefix(string prefix){
                prefixes.Add (prefix);
                return this;
            }


        }
    }
}

