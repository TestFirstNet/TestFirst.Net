using System.Collections;
using System.Collections.Generic;

namespace TestFirst.Net.Matcher.Internal
{
    /// <summary>
    /// Each matcher must match only once and must have a match. Depending on the config either all items must be matched or 
    /// additional unmatched items may exist. Order is not important
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal abstract class AbstractListInAnyOrder<T> : AList.AbstractListMatcher<T>, AList.IAcceptMoreMatchers<T>, IProvidePrettyTypeName
    {
        private readonly List<IMatcher<T>> m_matchers;
        private readonly bool m_failOnAdditionalItems;

        protected internal enum FailOnAdditionalItems
        {
            True,False
        }

        protected internal AbstractListInAnyOrder(FailOnAdditionalItems failOnAdditionalItems, string matcherName):base(matcherName)
        {
            m_matchers = new List<IMatcher<T>>();
            m_failOnAdditionalItems = failOnAdditionalItems==FailOnAdditionalItems.True;
        }

        protected internal AbstractListInAnyOrder(IEnumerable<IMatcher<T>> matchers, FailOnAdditionalItems failOnAdditionalItems, string matcherName):base(matcherName)
        {
            m_matchers = new List<IMatcher<T>>(matchers);
            m_failOnAdditionalItems = failOnAdditionalItems==FailOnAdditionalItems.True;
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
            var passed = true;
            var ctxt = new MatchContext(m_matchers);
            int pos = 0;
            for(; pos < list.Count; pos++)
            {
                var item = list[pos];
                var matchPassed = false;
                if(ctxt.MatchersRemain())
                {
                    if (m_failOnAdditionalItems)
                    {
                        matchPassed = ctxt.Matches(item, pos, diagnostics);
                    }
                    else
                    {
                        //prevent useless diagnostics messages appearing in output as we call the matchers
                        //repeatably as we try to find a match
                        var childDiag = diagnostics.NewChild();
                        matchPassed = ctxt.Matches(item, pos, childDiag);
                        if (matchPassed)
                        {
                            diagnostics.Value(childDiag);
                        }
                    }                     
                }
                 
                if(!matchPassed)
                {
                    if (m_failOnAdditionalItems)
                    {
                        passed = false;
                    }
                }
                if(!passed)
                {
                    var child = diagnostics.NewChild();
                    child.Value("position", pos);
                    child.Value("item", item);
                    diagnostics.MisMatched(child);
                    break;
                }
            }
            if (ctxt.MatchersRemain())
            {
                passed = false;
            }
            if (!passed)
            {
                if (!ctxt.MatchersRemain())
                {
                    diagnostics.Text("All matchers matched");
                }
                ctxt.AddMisMatchMessage(diagnostics);
                if (pos < list.Count)
                {
                    diagnostics.Child("Additional items not matched", SubSet(pos, list));
                }
            }                
            return passed;
        }

        private static IList SubSet(int startIndex, IList list)
        {
            var sub = new List<object>(list.Count - startIndex);
            for (int i = startIndex; i < list.Count; i++)
            {
                sub.Add(list[i]);
            }
            return sub;
        }

        public override string ToString()
        {
            var desc = new Description();
            DescribeTo(desc);
            return desc.ToString();
        }

        public override void DescribeTo(IDescription desc)
        {
            if (m_failOnAdditionalItems)
            {
                desc.Text("A List in any order containing only (" + m_matchers.Count + ") :");
            }
            else
            {
                desc.Text("A List in any order containing at least (" + m_matchers.Count + ") :");
            }
            desc.Children(m_matchers);
        }

        private class MatchContext
        {
            private readonly List<IMatcher<T>> m_remainingMatchers;

            internal MatchContext(IEnumerable<IMatcher<T>> matchers)
            {
                m_remainingMatchers = new List<IMatcher<T>>(matchers);
            }

            internal bool Matches(System.Object item, int itemPos, IMatchDiagnostics diagnostics)
            {
                //collect all the mismatches for later
                var children = new List<IMatchDiagnostics>(m_remainingMatchers.Count);
                foreach (var matcher in m_remainingMatchers)
                {
                    //want to keep non matchign matchers clear
                    var childDiag = diagnostics.NewChild();
                    if (childDiag.TryMatch(item, matcher))
                    {
                        diagnostics.Child(childDiag);
                        m_remainingMatchers.Remove(matcher);
                        return true;
                    }
                    children.Add(childDiag);
                }
                //lets print all the mis matches
                diagnostics.Children(children);
                return false;
            }

            internal bool MatchersRemain()
            {
                return m_remainingMatchers.Count > 0;
            }

            internal void AddMisMatchMessage(IMatchDiagnostics diagnostics)
            {
                if(MatchersRemain()){
                    diagnostics.Child("didn't match", m_remainingMatchers);
                }
            }
        }
    }
}