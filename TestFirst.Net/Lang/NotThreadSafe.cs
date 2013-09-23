using System;

namespace TestFirst.Net.Lang
{
    /// <summary>
    /// Explicitely marks a class as NOT Threadsafe. This is only used for documentation and has no runtime effects. It is assumed
    /// that all classes not marked with ThreadSafe are not threadsafe, but this could be due to in advertent omission, by using
    /// this attribute it makes this very clear.
    /// 
    /// If a class is mariked with this attribute, it does not mean two instances in different threads are not supported, in fact this
    /// is how classes should be written.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class NotThreadSafe : Attribute
    {
    }
}
