using System;

namespace TestFirst.Net
{
    /// <summary>
    /// Diagnostics which does not capture any test. This is for performance when the output is ignored
    /// </summary>
    public class NullMatchDiagnostics : NullDescription, IMatchDiagnostics
    {
        /// <summary>
        /// Keep a single instance around to remove the need to create it over and over when a single shared
        /// instance exihibits the exact same behaviour (provided nothing locks on this)
        /// </summary>
        public new static readonly NullMatchDiagnostics Instance = new NullMatchDiagnostics();

        public IMatchDiagnostics NewChild()
        {
            return this;
        }

        public bool TryMatch(Object actual, int index, IMatcher childMatcher)
        {
            return InternalTryMatch(actual,childMatcher);
        }

        public bool TryMatch<T>(T actual, int index, IMatcher<T> childMatcher)
        {
            return InternalTryMatch(actual, childMatcher);
        }

        public bool TryMatch(Object actual, IMatcher childMatcher)
        {
            return InternalTryMatch(actual, childMatcher);
        }

        public bool TryMatch(Object actual, String actualName, IMatcher childMatcher)
        {
            return InternalTryMatch(actual,childMatcher);
        }

        public bool TryMatch<T>(T actual, IMatcher<T> childMatcher)
        {
            return InternalTryMatch(actual, childMatcher);
        }

        public bool TryMatch<T>(T actual, String actualName, IMatcher<T> childMatcher)
        {
            return InternalTryMatch(actual, childMatcher);
        }

        public bool TryMatch<T>(T actual, IMatcher<T> childMatcher, ISelfDescribing selfDescribing)
        {
            return InternalTryMatch(actual, childMatcher);
        }

        public bool TryMatch(Object actual, IMatcher childMatcher, ISelfDescribing selfDescribing)
        {
            return InternalTryMatch(actual, childMatcher);
        }

        private bool InternalTryMatch(Object actual, IMatcher childMatcher)
        {
            return childMatcher.Matches(actual, this);
        }

        public IMatchDiagnostics Matched(string text, params object[] args)
        {
            return this;
        }

        public IMatchDiagnostics MisMatched(string text, params object[] args)
        {
            return this;
        }

        public IMatchDiagnostics Matched(ISelfDescribing selfDescribing)
        {
            return this;
        }

        public IMatchDiagnostics MisMatched(ISelfDescribing selfDescribing)
        {
            return this;
        }
    }
}
