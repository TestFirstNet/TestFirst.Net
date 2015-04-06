using System;

namespace TestFirst.Net {

    /// <summary>
    /// Conveneince class to use when usign matchers outside of a scenario
    /// </summary>
    public static class Expect {
        
        /// <summary>
        /// Build an expect with a label which is useful in loops when you want to know what the loop value was.
        /// 
        /// Usage:
        ///     
        /// foreach(var x in bar){
        ///     Expect.For(x)  -- this is simply printed on failure, not used for matching
        ///         .That(Something(x))
        ///         .Is(AFoo.With()...);
        /// }
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public static ExpectWithLabel For(Object label)
        {
            return new ExpectWithLabel(label);
        }

        /// <summary>
        /// Helper method to make using matchers outside of scenarios easier and cleaner
        /// 
        /// Usage:
        /// var x = Foo();
        /// Expect.That(x)
        ///     .Is(AFoo.With()...)
        ///     
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="actual"></param>
        /// <returns></returns>
        public static Expect<T> That<T>(T actual) {
            return new Expect<T>(actual);
        }
    }
    
    public class ExpectWithLabel
    {
        private readonly Object m_label;

        internal ExpectWithLabel(Object label)
        {
            m_label = label;
        }

        public Expect<T> That<T>(T actual)
        {
            return new Expect<T>(actual,m_label);
        }

    }
    public class Expect<T> {
        private readonly T m_actual;
        private readonly Object m_label;

        public Expect(T actual) {
            m_actual = actual;
            m_label = null;
        }

        public Expect(T actual,Object label) {
            m_actual = actual;
            m_label = label;
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

            if (m_label != null) {
                desc.Child("for", m_label);
            }

            desc.Child("expected", matcher);
            desc.Child("but was", m_actual);
            desc.Text("==== Diagnostics ====");
            desc.Child(diag);
            TestFirstAssert.Fail(Environment.NewLine + desc.ToString());
        }
    }
}