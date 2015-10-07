using System;

namespace TestFirst.Net.Lang
{
    /// <summary>
    /// Explicitly marks a class as NOT thread-safe. This is only used for documentation and has no runtime effects. It is assumed
    /// that all classes not marked with NotThreadSafe are not thread-safe, but this could be due to in advertent omission, by using
    /// this attribute it makes this very clear.
    /// <para>
    /// If a class is marked with this attribute, it does not mean two instances in different threads are not supported, in fact this
    /// is how classes should be written.
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class NotThreadSafe : Attribute
    {
    }
}
