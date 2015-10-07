using System;
using System.Collections.Generic;
using System.Linq;
using TestFirst.Net.Matcher.Internal;

namespace TestFirst.Net.Matcher
{
    public static class Matchers
    {
        public static IEnumerable<IMatcher<T>> MatchersFromValues<T, TVal>(Func<TVal, IMatcher<T>> func, IEnumerable<TVal> values)
        {
            return values.Select(func.Invoke).ToList();
        }

        public static IMatcher<T> Function<T>(Func<T, bool> matchFunc, string mismatchDesc)
        {
            return Function(matchFunc, () => mismatchDesc);
        }

        public static IMatcher<T> Function<T>(Func<T, bool> matchFunc, Func<string> mismatchMessageFactory)
        {
            return new FuncTypeName<T>(matchFunc, mismatchMessageFactory);
        }

        public static IMatcher<T> Function<T>(Func<T, bool> matchFunc, Action<IDescription> mismatchMessageFactory)
        {
            return new FuncTypeName<T>(matchFunc, mismatchMessageFactory);
        }

        public static IMatcher<TActual> Function<TActual>(Func<TActual, IMatchDiagnostics, bool> matchFunc, string mismatchDesc)
        {
            return Function(matchFunc, () => mismatchDesc);
        }

        public static IMatcher<TActual> Function<TActual>(Func<TActual, IMatchDiagnostics, bool> matchFunc, Func<string> mismatchMessageFactory)
        {
            return new FuncWithDiagnosticsTypeName<TActual>(matchFunc, mismatchMessageFactory);
        }

        public static IMatcher<TActual> Function<TActual>(Func<TActual, IMatchDiagnostics, bool> matchFunc, Action<IDescription> mismatchMessageFactory)
        {
            return new FuncWithDiagnosticsTypeName<TActual>(matchFunc, mismatchMessageFactory);
        }

        /// <summary>
        /// Returns a matcher which requires all of the provided matchers to
        /// match.
        /// </summary>
        /// <param name="matchers">the matchers to match. Not null</param>
        /// <typeparam name="T">The type of the object to match</typeparam>
        /// <returns>A matcher</returns>
        public static IMatcher<T> All<T>(params IMatcher<T>[] matchers)
        {
            if (matchers.Count() == 1)
            {
                return matchers[0];
            }
            return new AllMatchers<T>(matchers);
        }

        /// <summary>
        /// Returns a matcher which requires at least one of the provided matchers to
        /// match.
        /// </summary>
        /// <param name="matchers">the matchers to match. Not null</param>
        /// <typeparam name="T">The type of the object to match</typeparam>
        /// <returns>A matcher</returns>
        public static IMatcher<T> Any<T>(params IMatcher<T>[] matchers)
        {
            if (matchers.Count() == 1)
            {
                return matchers[0];
            }
            return new AnyMatchers<T>(matchers);
        }

        public static IMatcher<T> Not<T>(IMatcher<T> matcher)
        {
            return new NotMatcher<T>(matcher);
        }

        /// <summary>
        /// Convenience method to assert an instance matches the given matcher
        /// </summary>
        /// <param name="actual">the instance to pass to the matcher</param>
        /// <param name="matcher">the matcher to invoke</param>
        /// <typeparam name="T">The type of the instance to match</typeparam>
        public static void AssertMatch<T>(T actual, IMatcher<T> matcher)
        {
            matcher.AssertMatch(actual);
        }
        
        public static void AssertMatch(object actual, IMatcher matcher)
        {
            matcher.AssertMatch(actual);
        }

        public static bool Matches(object actual, IMatcher matcher)
        {
            return matcher.Matches(actual);            
        }

        private class FuncTypeName<T> : AbstractMatcher<T>, IProvidePrettyTypeName
        {
            private readonly Func<T, bool> m_matchFunc;
            private readonly Func<string> m_mismatchMessageFactoryGivesString;
            private readonly Action<IDescription> m_mismatchMessageFactoryTakesDesc;

            public FuncTypeName(Func<T, bool> matchFunc, Func<string> mismatchMessageFactoryGivesString)
            {
                if (matchFunc == null)
                {
                    throw new NullReferenceException("Expect a match function");
                }
                if (mismatchMessageFactoryGivesString == null)
                {
                    throw new NullReferenceException("Expect a message factory function returning a string");
                }
                m_matchFunc = matchFunc;
                m_mismatchMessageFactoryGivesString = mismatchMessageFactoryGivesString;
                m_mismatchMessageFactoryTakesDesc = null;
            }

            public FuncTypeName(Func<T, bool> matchFunc, Action<IDescription> mismatchMessageFactoryTakesDesc)
            {
                if (matchFunc == null)
                {
                    throw new NullReferenceException("Expect a match function");
                }
                if (mismatchMessageFactoryTakesDesc == null)
                {
                    throw new NullReferenceException("Expect a message factory action taking a description");
                }
                m_matchFunc = matchFunc;
                m_mismatchMessageFactoryGivesString = null;
                m_mismatchMessageFactoryTakesDesc = mismatchMessageFactoryTakesDesc;
            }

            public override bool Matches(T instance, IMatchDiagnostics diag)
            {
                return m_matchFunc.Invoke(instance);
            }

            public override void DescribeTo(IDescription description)
            {
                if (m_mismatchMessageFactoryTakesDesc != null)
                {
                    m_mismatchMessageFactoryTakesDesc.Invoke(description);
                }
                else if (m_mismatchMessageFactoryGivesString != null)
                {
                    description.Text(m_mismatchMessageFactoryGivesString.Invoke());
                }
                else
                {
                    description.Text("No description for " + GetType().Name + "@" + GetHashCode());
                }
            }

            public string GetPrettyTypeName()
            {
                return "Matchers.Function(" + ProvidePrettyTypeName.GetPrettyTypeNameFor<T>() + ")";
            }
        }

        private class FuncWithDiagnosticsTypeName<TActual> : AbstractMatcher<TActual>, IProvidePrettyTypeName
        {
            private readonly Func<TActual, IMatchDiagnostics, bool> m_matchFunc;

            private readonly Func<string> m_mismatchMessageFactoryGivesString;
            private readonly Action<IDescription> m_mismatchMessageFactoryTakesDesc;

            public FuncWithDiagnosticsTypeName(Func<TActual, IMatchDiagnostics, bool> matchFunc, Func<string> mismatchMessageFactoryGivesString)
            {
                if (matchFunc == null)
                {
                    throw new NullReferenceException("Expect a match function");
                }
                if (mismatchMessageFactoryGivesString == null)
                {
                    throw new NullReferenceException("Expect a message factory function giving a string");
                }

                m_matchFunc = matchFunc;
                m_mismatchMessageFactoryGivesString = mismatchMessageFactoryGivesString;
                m_mismatchMessageFactoryTakesDesc = null;
            }

            public FuncWithDiagnosticsTypeName(Func<TActual, IMatchDiagnostics, bool> matchFunc, Action<IDescription> mismatchMessageFactoryTakesDesc)
            {
                if (matchFunc == null)
                {
                    throw new NullReferenceException("Expect a match function");
                }
                if (mismatchMessageFactoryTakesDesc == null)
                {
                    throw new NullReferenceException("Expect a message factory action taking a description");
                }
                m_matchFunc = matchFunc;
                m_mismatchMessageFactoryGivesString = null;
                m_mismatchMessageFactoryTakesDesc = mismatchMessageFactoryTakesDesc;
            }

            public override bool Matches(TActual instance, IMatchDiagnostics diag)
            {
                return m_matchFunc.Invoke(instance, diag);
            }

            public override void DescribeTo(IDescription description)
            {
                if (m_mismatchMessageFactoryTakesDesc != null)
                {
                    m_mismatchMessageFactoryTakesDesc.Invoke(description);
                }
                else if (m_mismatchMessageFactoryGivesString != null)
                {
                    description.Text(m_mismatchMessageFactoryGivesString.Invoke());
                }
                else
                {
                    description.Text("No description for " + GetType().Name + "@" + GetHashCode());
                }
            }

            public string GetPrettyTypeName()
            {
                return "Matchers.Function(" + ProvidePrettyTypeName.GetPrettyTypeNameFor<TActual>() + ")";
            }
        }

        private class AllMatchers<T> : AbstractMatcher<T>
        {
            private readonly List<IMatcher<T>> m_matchers;
            public AllMatchers(IEnumerable<IMatcher<T>> matchers)
            {
                if (matchers == null)
                {
                    throw new NullReferenceException("Expect a non null list of matchers");
                }
                m_matchers = new List<IMatcher<T>>(matchers);
            }

            public override bool Matches(T instance, IMatchDiagnostics diag)
            {
                return m_matchers.All(matcher => matcher.Matches(instance, diag));
            }

            public override void DescribeTo(IDescription description)
            {
                description.Text("Match All");
                description.Children("matchers", m_matchers);
            }
        }

        private class AnyMatchers<T> : AbstractMatcher<T>
        {
            private readonly List<IMatcher<T>> m_matchers;
            public AnyMatchers(IEnumerable<IMatcher<T>> matchers)
            {
                if (matchers == null)
                {
                    throw new NullReferenceException("Expect a non null list of matchers");
                }
                m_matchers = new List<IMatcher<T>>(matchers);
            }

            public override bool Matches(T instance, IMatchDiagnostics diag)
            {
                return m_matchers.Any(matcher => matcher.Matches(instance, diag));
            }

            public override void DescribeTo(IDescription description)
            {
                description.Text("Match Any");
                description.Children("matchers", m_matchers);
            }
        }

        private class NotMatcher<T> : AbstractMatcher<T>
        {
            private readonly IMatcher<T> m_matcher;

            public NotMatcher(IMatcher<T> matcher)
            {
                m_matcher = matcher;
            }

            public override bool Matches(T actual, IMatchDiagnostics diag)
            {
                return !m_matcher.Matches(actual, diag);
            }

            public override void DescribeTo(IDescription description)
            {
                description.Child("Not", m_matcher);
            }
        }
    }
}
