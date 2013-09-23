using System;

namespace TestFirst.Net
{
    /// <summary>
    /// A typed matcher which can provide diagnostics information about what it matches and does not
    /// </summary>
    public interface IMatcher<in T> : IMatcher
    {
        bool Matches(T actual, IMatchDiagnostics diagnostics);
    }

    /// <summary>
    /// A matcher which can provide diagnostics information about what it matches and does not
    /// </summary>
    public interface IMatcher : ISelfDescribing, ISimpleMatcher
    {
        bool Matches(Object actual, IMatchDiagnostics diagnostics);
    }
}
