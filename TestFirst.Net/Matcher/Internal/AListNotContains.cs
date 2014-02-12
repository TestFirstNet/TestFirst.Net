using System.Collections;
using System.Collections.Generic;

namespace TestFirst.Net.Matcher.Internal
{
    /// <summary>
    /// Each matcher must match only once and must have a match. Depending on the config either all items must be matched or 
    /// additional unmatched items may exist. Order is not important
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class AListNotContains<T> : AList.AbstractListMatcher<T>, AList.IAcceptMoreMatchers<T>, IProvidePrettyTypeName
    {
        private readonly List<IMatcher<T>> m_matchers = new List<IMatcher<T>>();

        internal AListNotContains():base("AListNotContains")
        {
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
            int pos = 0;
            for (; pos < list.Count; pos++)
            {
                var item = list[pos];
                foreach (var matcher in m_matchers)
                {
                    //prevent useless diagnostics messages appearing in output as we call the matchers
                    //repeatably as we try to find a match
                    var childDiag = diagnostics.NewChild();
                    if (childDiag.TryMatch(item, matcher))
                    {
                        diagnostics.Value("position", pos);
                        diagnostics.Value("item", item);
                        diagnostics.MisMatched(childDiag);
                        return false;
                    }
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
            desc.Text("A List containing none of:");
            desc.Children(m_matchers);
        }
    }
}