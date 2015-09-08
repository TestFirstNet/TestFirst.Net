using System;
using NUnit.Framework;
using TestFirst.Net.Examples.Api.Query;
using TestFirst.Net.Examples.Service.Handler.Query;
using TestFirst.Net.Extensions.NUnit;
using TestFirst.Net.Matcher;

namespace TestFirst.Net.Examples.Service
{
    [TestFixture]
    public class ExampleAppTest : AbstractNUnitScenarioTest
    {
        [Test]
        public void RegisterHandler_NUnitStyle()
        {
            //given
            var app = new ExampleApp();
            app.RegisterHandler(new MyHandler());
            var result = new object();
            //when
            var response = app.Invoke(new MyQuery{ReturnMe = result});
            //then
            Assert.AreSame(result, response.Result);
        }

        [Test]
        public void RegisterHandler_TestFirstStyle()
        {
            ExampleApp app;
            Object result;
            MyQuery.Response response;

            Scenario()
                .Given(app = new ExampleApp())
                .Given(() => app.RegisterHandler(new MyHandler()))
                .Given(result = new Object())
                .When(response = app.Invoke(new MyQuery {ReturnMe = result}))
                .Then(ExpectThat(response), Is(AResponse.With().Result(result)));
        }

        private class AResponse : PropertyMatcher<MyQuery.Response>
        {
            //provide access to refactor safe property names
            private static readonly MyQuery.Response PropertyNames = null;

            public static AResponse With()
            {
                return new AResponse();
            }

            public AResponse Result(Object val)
            {
                Result(AnInstance.SameAs(val));
                return this;
            }

            public AResponse Result(IMatcher<Object> matcher)
            {
                WithProperty(() => PropertyNames.Result, matcher);
                return this;
            }
        }

        private class MyHandler : AbstractQueryHandler<MyQuery, MyQuery.Response>
        {            
            public override MyQuery.Response Handle(MyQuery query)
            {
                return new MyQuery.Response {Result = query.ReturnMe};
            }
        }

        private class MyQuery : IReturn<MyQuery.Response>
        {
            internal Object ReturnMe { get; set; }

            internal class Response
            {
                internal Object Result { get; set; }
            }
        }
    }
}
