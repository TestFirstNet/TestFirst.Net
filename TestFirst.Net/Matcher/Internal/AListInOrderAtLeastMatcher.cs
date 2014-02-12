using System;
using System.Collections;
using System.Collections.Generic;

namespace TestFirst.Net.Matcher.Internal
{
    /// <summary>
    /// There can be more items than matchers, matchers must apply in order, and each matcher must match. This means there may 
    /// be additional items which don't match
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class AListInOrderAtLeast<T> : AList.AbstractListMatcher<T>, AList.IAcceptMoreMatchers<T>, IProvidePrettyTypeName
    {
        private readonly List<IMatcher<T>> m_matchers;
 
        internal AListInOrderAtLeast(IEnumerable<IMatcher<T>> matchers):base("InOrderWithAtLeast")
        {
            //check each matcher is not null? or throw error when it happens
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
                diagnostics.MisMatched(Description.With().Value("items are null").Value("expected", "not null"));
                return false;
            }
            var items = AsEfficientList(actual); 
            var matcher = new MatchContext(m_matchers);
            if (!matcher.CanStillMatch())
            {
                //no matchers so all ok
                return true;
            }
            matcher.NextMatcher();
            Object lastMatchedItem = default(T); 
            int lastMatchedItemIdx = -1;
            for (int i = 0; i < items.Count && matcher.CanStillMatch(); i++)
            {
                var item = items[i];
                if (matcher.Matches(item, i, diagnostics))
                {
                    lastMatchedItem = item;
                    lastMatchedItemIdx = i;
                    matcher.NextMatcher();
                }
            }
            //check all matchers matched
            if (matcher.CanStillMatch())
            {
                matcher.AddMisMatchMessage(diagnostics, lastMatchedItem, lastMatchedItemIdx, items);
                return false;
            }
            return true;
        }

        private class MatchContext
        {
            private int m_currentMatcherIdx;
            private bool m_canStillMatch;
            private readonly List<IMatcher<T>> m_matchers;
                
            internal MatchContext(List<IMatcher<T>> matchers)
            {
                m_currentMatcherIdx = -1;
                m_canStillMatch = matchers.Count > 0;
                m_matchers = matchers;
            }

            internal bool Matches(Object item, int itemPos, IMatchDiagnostics diagnostics)
            {
                var matcher = CurrentMatcher();
                return diagnostics.TryMatch(item, itemPos, matcher);
            }

            private IMatcher<T> CurrentMatcher()
            {
                if (m_currentMatcherIdx < 0)
                {
                    throw new InvalidOperationException("no more matchers");
                }
                var matcher = m_matchers[m_currentMatcherIdx];
                if (matcher == null)
                {
                    throw new ArgumentException("Child matcher is null, at position " + m_currentMatcherIdx);
                }
                return matcher;
            }

            internal void NextMatcher()
            {
                if (HasNextMatcher())
                {
                    m_currentMatcherIdx++;
                }
                else
                {
                    m_canStillMatch = false;
                }
            }

            internal bool CanStillMatch()
            {
                return m_canStillMatch;
            }

            private bool HasNextMatcher()
            {
                return m_currentMatcherIdx < m_matchers.Count - 1;
            }

            internal void AddMisMatchMessage(IMatchDiagnostics diagnostics, Object lastMatchedItem, int itemPosition, IEnumerable items)
            {
                if( HasNextMatcher()){
                    diagnostics.Text("not all matchers matched, matchers which didn't match were");
                    diagnostics.Child("matchedUpToItemAtPosition", itemPosition);
                    diagnostics.Child("matchedUpToItem", lastMatchedItem);
                    diagnostics.Child("lastMatchingMatcher", CurrentMatcher());
                    var remaining = m_matchers.GetRange(m_currentMatcherIdx, m_matchers.Count - m_currentMatcherIdx); 
                    diagnostics.Children("remainingMatchers", remaining);
                    diagnostics.Children("items", items);
                }
            }
        }

        public override string ToString()
        {
            var desc = new Description();
            DescribeTo(desc);
            return desc.ToString();
        }

        public override void DescribeTo(IDescription desc)
        {
            desc.Text("A list in order containing at least:");
            desc.Children(m_matchers);
        }
    }
}