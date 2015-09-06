using System;

namespace TestFirst.Net.Web
{
    public abstract class AbstractHandler
    {
        private readonly Func<HttpListenerRequest, HttpListenerResponse,string> _func;

        public SimpleHandler(Func<HttpListenerRequest, HttpListenerResponse,string> func){
            this._func = func;
        }

        public void Handle (HttpListenerContext ctxt){

            var req = ctxt.Request;
            var res = ctxt.Response;
            try {
                var s = _func.Invoke(req, res);

                byte[] buf = Encoding.UTF8.GetBytes(s);
                res.ContentLength64 = buf.Length;
                res.OutputStream.Write(buf, 0, buf.Length);
            } catch (Exception e){ 
                Console.WriteLine("ctxt error:" + e.ToString());
            } finally {
                // always close the stream
                try {
                    res.OutputStream.Close ();
                } catch (Exception e) { 
                    Console.WriteLine ("stream close error:" + e.ToString ());
                }
            }
        }

        protected
    }
    }
}

