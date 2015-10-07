using System;

namespace TestFirst.Net.Lang
{
    /// <summary>
    /// Marks a class as intended to be thread-safe. This is only used for documentation and has no runtime effects. Note
    /// that this only marks intent, it is still up to developers to ensure the class meets the thread-safety requirements
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class ThreadSafe : Attribute
    {
        /// <summary>
        /// Gets or sets the caveats with regards to thread-safety. For example, only after certain lifecycles have been met,
        /// if certain methods are not called etc. Aim should be to not have any caveats of course.
        /// </summary>
        public string Caveats { get; set; }
    }
}
