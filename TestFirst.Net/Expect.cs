using System;

namespace TestFirst.Net {

    /// <summary>
    /// Conveneince class to use when usign matchers outside of a scenario
    /// </summary>
    public static class Expect {

        public static Expect<T> That<T>(T actual) {
            return new Expect<T>(actual);
        }
    }

    public class Expect<T> {
        private readonly T m_actual;

        public Expect(T actual) {
            m_actual = actual;
        }

        public void Is(IMatcher<T> matcher) {
            AssertMatches(matcher);
        }

        public void Is<TNull>(IMatcher<TNull?> matcher) 
        where TNull : struct, T
        {
            AssertMatches(matcher);
        }

        public void And(IMatcher<T> matcher) {
            AssertMatches(matcher);
        }

        public void And<TNull>(IMatcher<TNull?> matcher)
        where TNull : struct, T
        {
            AssertMatches(matcher);
        }

        private void AssertMatches(IMatcher<T> matcher) {
            var diag = new MatchDiagnostics();
            if (!matcher.Matches(m_actual, diag))
            {
                GenerateAndThrowFailMsg(matcher, diag);
            }
        }

        private void AssertMatches<TNull>(IMatcher<TNull?> matcher)
        where TNull : struct, T
        {
            var diag = new MatchDiagnostics();
            if (!matcher.Matches(m_actual, diag)) 
            {
                GenerateAndThrowFailMsg(matcher, diag);
            }
        }

        private void GenerateAndThrowFailMsg(IMatcher matcher, MatchDiagnostics diag)
        {
            var desc = new Description();

            desc.Child("expected", matcher);
            desc.Child("but was", m_actual);
            desc.Text("==== Diagnostics ====");
            desc.Child(diag);
            TestFirstAssert.Fail(Environment.NewLine + desc.ToString());
        }
    }
}