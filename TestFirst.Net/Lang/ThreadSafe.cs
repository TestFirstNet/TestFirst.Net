using System;

namespace TestFirst.Net.Lang
{
    /// <summary>
    /// Marks a class as intended to be Threadsafe. This is only used for documentation and has no runtime effects. Note
    /// that this only marks intent, it is still up to developers to ensure the class meets the threadsafety requirements
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class ThreadSafe : Attribute
    {
        /// <summary>
        /// Allows addition of any caveats with regards to threadsafety. For example, only after certain lifecycles have been met,
        /// if certain methods are not called etc. Aim should be to not have any caveats ofcourse.
        /// </summary>
        public String Caveats { get; set; }
    }
}
