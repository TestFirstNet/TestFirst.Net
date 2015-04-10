using System;

namespace TestFirst.Net
{

    public interface IMatchDiagnostics:IDescription
    {
        IMatchDiagnostics Matched(String text, params Object[] args);
        IMatchDiagnostics Matched(ISelfDescribing selfDescribing);
           
        IMatchDiagnostics MisMatched(String text, params Object[] args);
        IMatchDiagnostics MisMatched(ISelfDescribing selfDescribing);

        /// <summary>
        /// Use the given matcher to match the given input.  Attempts to pretty format the response
        /// </summary>
        /// <returns><c>true</c>, if match was successfull, <c>false</c> otherwise.</returns>
        /// <param name="actual">Object to match</param>
        /// <param name="actualIndex">On failure include this as the index for the object</param>
        /// <param name="childMatcher">The matcher to use against the provided object</param>
        bool TryMatch(Object actual, int actualIndex, IMatcher childMatcher);

        /// <summary>
        /// Use the given matcher to match the given input.  Attempts to pretty format the response
        /// </summary>
        /// <returns><c>true</c>, if match was successfull, <c>false</c> otherwise.</returns>
        /// <param name="actual">Object to match</param>
        /// <param name="actualIndex">On failure include this as the index for the object</param>
        /// <param name="childMatcher">The matcher to use against the provided object</param>
        bool TryMatch<T>(T actual, int actualIndex, IMatcher<T> childMatcher);

        /// <summary>
        /// Use the given matcher to match the given input.  Attempts to pretty format the response
        /// </summary>
        /// <returns><c>true</c>, if match was successfull, <c>false</c> otherwise.</returns>
        /// <param name="actual">Object to match</param>
        /// <param name="childMatcher">The matcher to use against the provided object</param> 
        bool TryMatch(Object actual, IMatcher childMatcher);

        /// <summary>
        /// Use the given matcher to match the given input.  Attempts to pretty format the response
        /// </summary>
        /// <returns><c>true</c>, if match was successfull, <c>false</c> otherwise.</returns>
        /// <param name="actual">Object to match</param>
        /// <param name="childMatcher">The matcher to use against the provided object</param> 
        bool TryMatch<T>(T actual, IMatcher<T> childMatcher);

        /// <summary>
        /// Use the given matcher to match the given input.  Attempts to pretty format the response
        /// </summary>
        /// <returns><c>true</c>, if match was successfull, <c>false</c> otherwise.</returns>
        /// <param name="actual">Object to match</param>
        /// <param name="actualLabel">On failure include this as a label for the object</param>
        /// <param name="childMatcher">The matcher to use against the provided object</param>
        bool TryMatch(Object actual, String actualLabel, IMatcher childMatcher);

        /// <summary>
        /// Use the given matcher to match the given input.  Attempts to pretty format the response
        /// </summary>
        /// <returns><c>true</c>, if match was successfull, <c>false</c> otherwise.</returns>
        /// <param name="actual">Object to match</param>
        /// <param name="actualLabel">On failure include this as a label for the object</param>
        /// <param name="childMatcher">The matcher to use against the provided object</param>
        bool TryMatch<T>(T actual, String actualLabel, IMatcher<T> childMatcher);

        bool TryMatch(Object actual, IMatcher childMatcher, ISelfDescribing selfDescribing);
        bool TryMatch<T>(T actual, IMatcher<T> childMatcher, ISelfDescribing selfDescribing);

        /// <summary>
        /// Return a child diagnostics context. This allows for performing sub matches and optionally
        /// including the match/mismatch messages. 
        /// 
        /// The return type is the same type as the current diagnostics. 
        /// </summary>
        /// <returns>A new child (or the same instance in the case of a null diagnostics)</returns>
        IMatchDiagnostics NewChild();
    }
}
