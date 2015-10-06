namespace TestFirst.Net
{
    /// <summary>
    /// A typed matcher which can provide diagnostics information about what it matches and does not
    /// </summary>
    /// <typeparam name="T">The type to match against</typeparam>
    public interface IMatcher<in T> : IMatcher
    {
        bool Matches(T actual, IMatchDiagnostics diagnostics);
    }

    /// <summary>
    /// A matcher which can provide diagnostics information about what it matches and does not
    /// </summary>
    public interface IMatcher : ISelfDescribing, ISimpleMatcher
    {
        bool Matches(object actual, IMatchDiagnostics diagnostics);
    }
}
