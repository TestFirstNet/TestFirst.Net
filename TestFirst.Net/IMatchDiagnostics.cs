using System;

namespace TestFirst.Net
{
    public interface IMatchDiagnostics:IDescription
    {
        IMatchDiagnostics Matched(String text, params Object[] args);
        IMatchDiagnostics Matched(ISelfDescribing selfDescribing);
           
        IMatchDiagnostics MisMatched(String text, params Object[] args);
        IMatchDiagnostics MisMatched(ISelfDescribing selfDescribing);

        bool TryMatch(Object actual, int index, IMatcher childMatcher);
        bool TryMatch<T>(T actual, int index, IMatcher<T> childMatcher);

        bool TryMatch(Object actual, IMatcher childMatcher);
        bool TryMatch<T>(T actual, IMatcher<T> childMatcher);

        /// <summary>
        /// Use the given matcher to match the given inpout.  Attempts to pretty format the response
        /// </summary>
        /// <returns>if the child matcher matched</returns>
        bool TryMatch(Object actual, String actualName, IMatcher childMatcher);
        bool TryMatch<T>(T actual, String actualName, IMatcher<T> childMatcher);

        bool TryMatch(Object actual, IMatcher childMatcher, ISelfDescribing selfDescribing);
        bool TryMatch<T>(T actual, IMatcher<T> childMatcher, ISelfDescribing selfDescribing);

        IMatchDiagnostics NewChild();
    }
}
