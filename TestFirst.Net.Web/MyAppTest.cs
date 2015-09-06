using System;
using TestFirst.Net.Web;
using NUnit.Framework;
using System.Threading;
using System.Data;
using System.Text;
using System.Collections.Generic;

namespace TestFirst.Net.Web
{
    [TestFixture]
    public class MyAppTest :AbstractWebScenarioTest 
    {

        [Test]
        public void test1(){
            AppServer server;
            MyApp app;
            Scenario ("test")
                .Given(server = A(AppServer.With()
                    .Defaults()
                    .Handler("app/home",ctxt=>HtmlPage.With()
                        .Header(h=>{
                            h.Title("Home Page")
                             .ScriptSrc("alert(\"home page!\");");
                        })
                        .Body("Welcome,home page"))
                    .Handler("app/**",ctxt=>HtmlPage.With().Title("hello world").Body("hello world"))
                ))
                .Given(app = MyApp.With().BaseUrl(server))
                .When(()=>app.LoadHomePage())
                .Then(Pause())
                ;
        }
    }

    public class HtmlPage {
        private string _header = "<head><title>no title</title></head>";
        private string _body = "<body>no-body</body>";

        public static HtmlPage With(){
            return new HtmlPage ();
        }

        public HtmlPage Title(string s){
            Header("<head><title>{0}</title></head>",s);
            return this;
        }

        public HtmlPage Header(Action<HtmlHeader> headerBuilder){
            var h = HtmlHeader.With ();
            headerBuilder (h);
            Header (h.ToString());
            return this;
        }

        public HtmlPage Header(string s,params object[] args){
            _header = SafeFormat(s,args);
            return this;
        }


        public HtmlPage Body(string s,params object[] args){
            _body = string.Format("<body>{0}</body>",string.Format(s,args));
            return this;
        }

        private static string SafeFormat(string s,params object[] args){
            if (args != null && args.Length > 0) {
                return String.Format (s, args);
            } else {
                return s;
            }
        }

        public override string ToString ()
        {
            var sb = new StringBuilder();
            sb.Append ("<html>");
            sb.Append (_header);
            sb.Append (_body);
            sb.Append ("</html>");
            return sb.ToString();
        }
    }


    public class HtmlHeader {
        private string _title = "no title";
        private IDictionary<string,string> _meta = new Dictionary<string,string>();
        private IDictionary<string,string> _scripts = new Dictionary<string,string>();

        public static HtmlHeader With(){
            return new HtmlHeader ();
        }

        public HtmlHeader Title(string s,params object[] args){
            _title = SafeFormat(s,args);
            return this;
        }

        public HtmlHeader Meta(string name,string value){
            _meta.Add (name, value);
            return this;
        }

        public HtmlHeader ScriptUrl(string url){
            _scripts.Add (url, null);
            return this;
        }

        public HtmlHeader ScriptSrc(string src){
            _scripts.Add (Guid.NewGuid().ToString(), src);
            return this;
        }

        private static string SafeFormat(string s,params object[] args){
            if (args != null && args.Length > 0) {
                return String.Format (s, args);
            } else {
                return s;
            }
        }

        public override string ToString ()
        {
            var sb = new StringBuilder();
            sb.AppendLine ("<head>");
            sb.Append ("<title>");
            sb.Append (_title);
            sb.AppendLine ("</title>");
            foreach (var kv in _meta) {
                sb.AppendLine ("<meta name=\"" + kv.Key + "\" value=\"" + kv.Value + "\" />");
            }
            foreach (var kv in _scripts) {
                if (kv.Value != null) {
                    sb.AppendLine ("<script rel=\"javascript\">" + kv.Value + "\n</script>");
                } else {
                    sb.AppendLine ("<script rel=\"javascript\" src=\"" + kv.Key + "\"></script>");
                }
            }
            sb.Append ("</head>");
            return sb.ToString();
        }
    }
}

