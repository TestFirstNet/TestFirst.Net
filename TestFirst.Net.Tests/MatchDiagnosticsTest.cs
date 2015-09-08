using System;
using System.Text;
using NUnit.Framework;

namespace TestFirst.Net.Test
{
    [TestFixture]
    public class MatchDiagnosticsTest
    {
        private static readonly String Indent = Description.Indent;

        [Test]
        public void AppendMatchedFieldTest()
        {
            var diag = new MatchDiagnostics()
                .Matched(Description.With().Value("on field","field1").Value("actual","fieldValue1"))
                .Matched(Description.With().Value("on field","field2").Value("actual","fieldValue2"))
                .ToString();

            var expect = new StringBuilder();
            expect.AppendLine("Match");
            expect.AppendLine(Indent + "on field:field1");
            expect.AppendLine(Indent + "actual:fieldValue1");
            expect.AppendLine("Match");
            expect.AppendLine(Indent + "on field:field2");
            expect.AppendLine(Indent + "actual:fieldValue2");

            AssertEquals(expect.ToString(), diag);
        }

        [Test]
        public void TryMatchPassTest()
        {
            var diag = new MatchDiagnostics();
            var matcher = new MyMatcher("MyValue");
            var matched = diag.TryMatch("MyValue", "MyProperty", matcher);

            Assert.IsTrue(matched, "Expected matcher to match");

            var expect = new StringBuilder();
            expect.AppendLine("Match");
            expect.AppendLine(Indent + "named:MyProperty");
            expect.AppendLine(Indent + "Equals:MyValue");
            AssertEquals(expect.ToString(), diag.ToString());
        }

        [Test]
        public void TryMatchNestedPassTest()
        {
            var diag = new MatchDiagnostics();

            var childMatcher = new MyMatcher("MyValue");
            var rootMatcher = new MyNestedMatcher<string>(childMatcher);
            var matched = diag.TryMatch("MyValue", "MyProperty", rootMatcher);

            Assert.IsTrue(matched, "Expected matcher to match");

            var expect = new StringBuilder();
            expect.AppendLine("Match");
            expect.AppendLine(Indent + "named:MyProperty");
            expect.AppendLine(Indent + "matches:");
            expect.AppendLine(Indent + Indent + "Equals:MyValue");

            AssertEquals(expect.ToString(), diag.ToString());
        }

        [Test]
        public void TryMatchFailTest()
        {
            var diag = new MatchDiagnostics();
            var matcher = new MyMatcher("MyValue");
            var matched = diag.TryMatch("MyWrongValue", "MyProperty", matcher);

            Assert.IsFalse(matched, "Expected matcher to _not_ match");

            var expect = new StringBuilder();
            expect.AppendLine("Mismatch!");
            expect.AppendLine(Indent + "named:MyProperty");
            expect.AppendLine(Indent + "matcherType:" + matcher.GetType());
            expect.AppendLine(Indent + "expected:" );
            expect.AppendLine(Indent + Indent + "Equals:MyValue" );
            expect.AppendLine(Indent + "but was (string,length 12,quoted):" );
            expect.AppendLine(Indent + Indent + "'MyWrongValue'" );
            expect.AppendLine(Indent + "Mismatch!");
            expect.AppendLine(Indent + Indent + "expected:MyValue");
            expect.AppendLine(Indent + Indent + "actual:MyWrongValue");


            AssertEquals(expect.ToString(), diag.ToString());
        }

        private void AssertEquals(String expect, String actual)
        {
            Assert.AreEqual(expect.Trim(), actual, "Not equal, actual '{}', got '{}'", new object[]{actual,expect});
        }

        class MyMatcher : AbstractMatcher<String>
        {
            private readonly string m_expectVal;

            public MyMatcher(string expectVal)
            {
                m_expectVal = expectVal;
            }

            public override void DescribeTo(IDescription description)
            {
                description.Value("Equals", m_expectVal);
            }

            public override bool Matches(string actual, IMatchDiagnostics diag)
            {
                if (m_expectVal.Equals(actual))
                {
                    diag.Matched(Description.With().Value("value", actual));
                    return true;
                }
                diag.MisMatched(Description.With().Value("expected", m_expectVal).Value("actual", actual));
                return false;
            }
        } 

        class MyNestedMatcher<T> : AbstractMatcher<T>
        {
            private readonly IMatcher<T> m_childMatcher; 

            public MyNestedMatcher(IMatcher<T> childMatcher)
            {
                m_childMatcher = childMatcher;
            }

            public override void DescribeTo(IDescription description)
            {
                description.Child("matches",m_childMatcher);
            }

            public override bool Matches(T actual, IMatchDiagnostics diag)
            {
                return diag.TryMatch(actual, "MyField", m_childMatcher);
            }
        } 

    }
}
