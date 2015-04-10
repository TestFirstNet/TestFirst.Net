using System;
using TestFirst.Net.Matcher.Internal;

namespace TestFirst.Net
{
    public class MatchDiagnostics : Description, IMatchDiagnostics
    {
        public IMatchDiagnostics NewChild()
        {
            return new MatchDiagnostics();
        }

        public MatchDiagnostics()
        {}

        public MatchDiagnostics(IAppendListener listener):base(listener)
        {}

        public bool TryMatch(Object actual, int index, IMatcher childMatcher)
        {
            return InternalTryMatch(actual,childMatcher, Description.With().Value("at position", index));
        }

        public bool TryMatch<T>(T actual, int index, IMatcher<T> childMatcher)
        {
            return InternalTryMatch(actual, childMatcher, Description.With().Value("at position", index));
        }

        public bool TryMatch(Object actual, IMatcher childMatcher)
        {
            return InternalTryMatch(actual, childMatcher, null);
        }

        public bool TryMatch(Object actual, String actualName, IMatcher childMatcher)
        {
            return InternalTryMatch(actual,childMatcher, Description.With().Value("named", actualName));
        }

        public bool TryMatch<T>(T actual, IMatcher<T> childMatcher)
        {
            return InternalTryMatch(actual, childMatcher, null);
        }

        public bool TryMatch<T>(T actual, String actualName, IMatcher<T> childMatcher)
        {
            return InternalTryMatch(actual, childMatcher, Description.With().Value("named", actualName));
        }

        public bool TryMatch<T>(T actual, IMatcher<T> childMatcher, ISelfDescribing selfDescribing)
        {
            return InternalTryMatch(actual, childMatcher, selfDescribing);
        }

        public bool TryMatch(Object actual, IMatcher childMatcher, ISelfDescribing selfDescribing)
        {
            return InternalTryMatch(actual, childMatcher, selfDescribing);
        }

        private bool InternalTryMatch(Object actual, IMatcher childMatcher, ISelfDescribing selfDescribing)
        {
            //so we can print child diagnostics after
            var childDiag = new MatchDiagnostics();
            var matched = childMatcher.Matches(actual, childDiag);
            
            Text(matched ? "Match":"Mismatch!");

            var desc = Description.With();
            if (selfDescribing != null)
            {
                desc.Value(selfDescribing);
            }
            if (!matched)
            {
                Expect.PrintExpectButGot(desc, actual, childMatcher);
                desc.Value(childDiag);
            }
            else
            {
                desc.Value(childMatcher);
            }   
            Child(desc);
            return matched;
        }

        public IMatchDiagnostics Matched(string text, params object[] args)
        {
            Matched(Description.With().Text(text,args));
            return this;
        }

        public IMatchDiagnostics MisMatched(string text, params object[] args)
        {
            MisMatched(Description.With().Text(text,args));
            return this;
        }

        public IMatchDiagnostics Matched(ISelfDescribing selfDescribing)
        {
            Text("Match");
            Child(selfDescribing);
            return this;
        }

        public IMatchDiagnostics MisMatched(ISelfDescribing selfDescribing)
        {
            Text("Mismatch!");
            Child(selfDescribing);
            return this;
        }
    }
}
