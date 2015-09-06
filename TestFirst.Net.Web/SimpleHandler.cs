using System;
using System.Net;
using System.Text;

namespace TestFirst.Net.Web
{
    public class SimpleHandler {

        private readonly Func<HttpListenerContext,object> _func;

        public SimpleHandler(Func<HttpListenerContext,object> func){
            this._func = func;
        }

        public void Handle (HttpListenerContext ctxt){

            var req = ctxt.Request;
            var res = ctxt.Response;
            try {
                var val = _func.Invoke(ctxt);
                string s = "";
                if(val != null){
                    //todo:handle various stream types or delegate?
                    s = val.ToString();
                }
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
    }
}

