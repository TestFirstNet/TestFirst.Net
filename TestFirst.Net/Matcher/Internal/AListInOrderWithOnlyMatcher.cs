using System.Collections;
using System.Collections.Generic;

namespace TestFirst.Net.Matcher.Internal
{
    /// <summary>
    /// The number of items must equal the number of matchers, and each matcher must match in order
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class AListInOrderWithOnlyMatcher<T> : AList.AbstractListMatcher<T>, AList.IAcceptMoreMatchers<T>, IProvidePrettyTypeName
    {
        private readonly List<IMatcher<T>> m_matchers;

        internal AListInOrderWithOnlyMatcher(IEnumerable<IMatcher<T>> matchers):base("InOrderWithOnly")
        {
            m_matchers = new List<IMatcher<T>>(matchers);
        }

        public AList.IAcceptMoreMatchers<T> And(IMatcher<T> matcher)
        {
            m_matchers.Add(matcher);
            return this;
        }

        public override bool Matches(IEnumerable actual, IMatchDiagnostics diagnostics)
        {
            if (actual == null)
            {
                diagnostics.MisMatched("items are null");
                return false;
            }
            var list = AsEfficientList(actual);
            if (m_matchers.Count != list.Count)
            {
                diagnostics
                    .Value("num items")
                    .Value("expected", m_matchers.Count)
                    .Value("actual", list.Count);
                return false;
            }
            for (int i = 0; i < m_matchers.Count; i++)
            {
                var matcher = m_matchers[i];
                var item = list[i];

                if (!diagnostics.TryMatch(item, i, matcher))
                {
                    return false;
                }
            }
            return true;
        }

        public override string ToString()
        {
            var desc = new Description();
            DescribeTo(desc);
            return desc.ToString();
        }
            
        public override void DescribeTo(IDescription desc)
        {
            desc.Text("A List in order containing only (" + m_matchers.Count + ") :");
            desc.Children(m_matchers);
        }
    }
}