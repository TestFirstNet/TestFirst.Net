using System;

namespace TestFirst.Net.Web
{
    public class MyApp: AbstractPageObject
    {
        private string _baseUrl;

        public MyApp (string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public static MyApp With(){
            return new MyApp("http://localhost/app");
        }

        public MyApp BaseUrl(AppServer server)
        {
            BaseUrl(server.BaseUrl);
            return this;
        }

        public MyApp BaseUrl(string baseUrl)
        {
            _baseUrl = baseUrl;
            return this;
        }

        public MyApp LoadHomePage(){


            return this;
        }
            
    }
}

