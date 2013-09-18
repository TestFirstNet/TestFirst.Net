using System.Collections.Generic;

namespace TestFirst.Net.Matcher.Internal
{
    ///<summary>
    /// The number of items must at least equal the number of matchers, and each matcher must match only once. Order is not important
    /// </summary>
    internal class AListInAnyOrderWithAtLeast<T> : AbstractListInAnyOrder<T>
    {
        internal AListInAnyOrderWithAtLeast(IEnumerable<IMatcher<T>> matchers) : base(matchers, FailOnAdditionalItems.False, "InAnyOrderWithAtLeast")
        {
        }
    }
}