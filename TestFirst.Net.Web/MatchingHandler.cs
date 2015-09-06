using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Collections;

namespace TestFirst.Net.Web
{
    public class MatchingHandler
    {
        private readonly Holder[] handlers;
        private readonly Handler defaultNotFound;

        public MatchingHandler (IDictionary<Regex,Handler> handlers,Handler defaultNotFound = null)
        {
            this.handlers = handlers.Select (kv => new Holder (kv.Key, kv.Value)).ToArray();
            this.defaultNotFound = defaultNotFound != null ? defaultNotFound : NewDefaultNotFound ();
        }

        private static Handler NewDefaultNotFound(){
            return new SimpleHandler((ctxt)=>{
                ctxt.Response.StatusCode = 404;

                return "<html><head><title></title></head><body>ERROR:404:NOT_FOUND:" + ctxt.Request.Url.ToString() + "</body></html>";

            }).Handle;
        }

        public void Handle(HttpListenerContext ctxt){
            var path = ctxt.Request.Url.AbsolutePath;
            for (var i = 0; i < handlers.Length; i++) {
                var holder = handlers[i];
                if (holder.Re.IsMatch (path)) {
                    holder.Handler (ctxt);
                    return;
                }
            }
            defaultNotFound (ctxt);
        }


        private class Holder {
            public readonly Regex Re;
            public readonly Handler Handler;

            public Holder(Regex re, Handler handler){
                this.Re = re;
                this.Handler = handler;
            }
        
        }



    }
}

