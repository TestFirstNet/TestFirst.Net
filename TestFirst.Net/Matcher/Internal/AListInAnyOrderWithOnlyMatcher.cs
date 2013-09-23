using System.Collections.Generic;

namespace TestFirst.Net.Matcher.Internal
{
    /// <summary>
    /// The number of items must equal the number of matchers, and each matcher must match only once. Order is not important
    /// </summary>
    internal class AListInAnyOrderWithOnly<T> : AbstractListInAnyOrder<T>
    {
        internal AListInAnyOrderWithOnly(IEnumerable<IMatcher<T>> matchers) : base(matchers, FailOnAdditionalItems.True, "InAnyOrderWithOnly")
        {
        }

        internal AListInAnyOrderWithOnly() : base(FailOnAdditionalItems.True, "InAnyOrderWithOnly")
        {
        }
    }
}