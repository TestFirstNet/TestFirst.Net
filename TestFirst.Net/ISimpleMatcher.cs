using System;

namespace TestFirst.Net
{
    public interface ISimpleMatcher<T> : ISimpleMatcher
    {
        bool Matches(T actual);
    }
    /// <summary>
    /// A matcher which does not provide any diganostics on why a match passed or failed
    /// </summary>
    public interface ISimpleMatcher
    {
        bool Matches(Object actual);
    }
}
