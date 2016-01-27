using System;
using TestFirst.Net.Matcher;
using TestFirst.Net.Matcher.Internal;

namespace TestFirst.Net 
{
    /// <summary>
    /// Convenience class to use when using matchers outside of a scenario
    /// </summary>
    public static class Expect 
    {
        /// <summary>
        /// Build an expect with a label which is useful in loops when you want to know what the loop value was.
        /// <para>
        /// Usage:
        /// </para>
        /// <para>
        /// foreach (var x in bar){
        ///     Expect.For("x=" + x)  -- this is simply printed on failure, not used for matching
        ///         .That(Something(x))
        ///         .Is(AFoo.With()...);
        /// }
        /// </para>
        /// </summary>
        /// <param name="label">Label to print on failure</param>
        /// <returns>Returns the expectation</returns>
        public static ExpectWithLabel For(object label)
        {
            return new ExpectWithLabel(label);
        }

        /// <summary>
        /// Helper method to make using matchers outside of scenarios easier and cleaner
        /// <para>
        /// Usage:
        /// </para>    
        /// <pre>
        /// var x = Foo();
        /// Expect.That(x)
        ///     .Is(AFoo.With()...)
        /// </pre>
        /// </summary>
        /// <typeparam name="T">The type of object to make an expectation about</typeparam>
        /// <param name="actual">The value to make an expectation about</param>
        /// <returns>An expectation of type T</returns>
        public static Expect<T> That<T>(T actual) 
        {
            return new Expect<T>(actual);
        }

        /// <summary>
        /// Usage:
        /// <pre>
        /// Expect.That(() => Foo.MyThrowsMethod()).Throws(AnException.With().Message("something"));
        /// </pre>
        /// </summary>
        /// <param name="action">The action which is expected to throw an exception</param>
        /// <returns>Expectation throws builder</returns>
        public static ExpectThrows That(Action action)
        {
            return new ExpectThrows(action);
        }

        public static void PrintExpectButGot(IDescription desc, object actual, IMatcher matcher)
        {
            if (desc.IsNull) 
            {
                return;
            }
            desc.Value("matcherType", GetMatcherType(matcher));
            desc.Child("expected", matcher);
            var s = actual as string;
            if (s != null)
            {
                var len = s.Length;
                if (len == 0) 
                {
                    desc.Child("but was (empty string, quoted)", "'" + actual + "'");
                } 
                else if (s.Trim().Length == 0)
                {
                    desc.Child("but was (blank string, length " + len + ", quoted)", "'" + actual + "'");
                } 
                else 
                {
                    desc.Child("but was (string, length " + len + ", quoted)", "'" + actual + "'");
                }
            } 
            else 
            {
                desc.Child("but was", actual);
            }
        }

        private static string GetMatcherType(IMatcher matcher)
        {
            var prettyfier = matcher as IProvidePrettyTypeName;
            if (prettyfier != null)
            {
                return prettyfier.GetPrettyTypeName();
            }
            return matcher.GetType().FullName;
        }
    }

    /// <summary>
    /// Adds additional information to the failure message
    /// </summary>
    public class ExpectWithLabel
    {
        private readonly object m_label;

        internal ExpectWithLabel(object label)
        {
            m_label = label;
        }

        public Expect<T> That<T>(T actual)
        {
            return new Expect<T>(actual, m_label);
        }

        public ExpectThrows That(Action action)
        {
            return new ExpectThrows(action);
        }
    }

    public class ExpectThrows : BaseExpect 
    {
        private readonly Action action;

        public ExpectThrows(Action action)
        {
            this.action = action;
        }

        public void Throws(IMatcher<Exception> matcher)
        {
            Exception thrown = null;
            try
            {
                action();
            } 
            catch (Exception e)
            {
                thrown = e;
            }
            MatchOrFail(thrown, matcher, null);
        }
    }

    public class Expect<T> : BaseExpect 
    {
        private readonly T m_actual;
        private readonly object m_label;

        public Expect(T actual) 
        {
            m_actual = actual;
            m_label = null;
        }

        public Expect(T actual, object label) 
        {
            m_actual = actual;
            m_label = label;
        }

        public Expect<T> IsNull()
        {
            AssertMatches(AnInstance.Null());
            return this;
        }

        public Expect<T> IsNotNull()
        {
            Is(AnInstance.NotNull<T>());
            return this;
        }

        public Expect<T> IsEqualTo(T expect)
        {
            Is(AnInstance.EqualTo(expect));
            return this;
        }

        public Expect<T> Is(IMatcher<T> matcher)
        {
            AssertMatches(matcher);
            return this;
        }

        public Expect<T> Is<TNull>(IMatcher<TNull?> matcher) 
            where TNull : struct, T
        {
            AssertMatches(matcher);
            return this;
        }

        public Expect<T> And(IMatcher<T> matcher)
        {
            AssertMatches(matcher);
            return this;
        }

        public Expect<T> And<TNull>(IMatcher<TNull?> matcher)
            where TNull : struct, T
        {
            AssertMatches(matcher);
            return this;
        }

        private void AssertMatches(IMatcher matcher) 
        {
            MatchOrFail(m_actual, matcher, m_label);
        }

        private void AssertMatches<TNull>(IMatcher<TNull?> matcher)
            where TNull : struct, T
        {
            MatchOrFail(m_actual, matcher, m_label);
        }
    }

    public class BaseExpect 
    {
        protected void MatchOrFail(object actual, IMatcher matcher, object label)
        {
            var diag = new MatchDiagnostics();

            if (!matcher.Matches(actual, diag)) 
            {
                GenerateAndThrowFailMsg(actual, matcher, diag, label);
            }
        }

        private void GenerateAndThrowFailMsg(object actual, IMatcher matcher, MatchDiagnostics diag, object label)
        {
            var desc = new Description();

            if (label != null) 
            {
                desc.Child("for", label);
            }
            Expect.PrintExpectButGot(desc, actual, matcher);
            desc.Text("==== Diagnostics ====");
            desc.Child(diag);
            TestFirstAssert.Fail(Environment.NewLine + desc);
        }
    }
}