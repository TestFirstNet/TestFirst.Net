using TestFirst.Net.Util;

namespace TestFirst.Net.Matcher
{
    public static class ADecimal
    {
        public static IMatcher<decimal?> EqualTo(decimal? expect)
        {
            if (expect == null)
            {
                return Null();
            }
            return Matchers.Function((decimal? actual) => actual == expect, "a decimal == " + expect.ToPrettyString());
        }

        public static IMatcher<decimal?> GreaterThan(decimal expect)
        {
            return Matchers.Function((decimal? actual) => actual!=null && expect.SafeCompareTo(actual) < 0, "a decimal > " + expect.ToPrettyString());
        }

        public static IMatcher<decimal?> GreaterOrEqualTo(decimal expect)
        {
            return Matchers.Function((decimal? actual) => actual!=null && expect.SafeCompareTo(actual) <= 0, "a decimal >= " + expect.ToPrettyString());
        }

        public static IMatcher<decimal?> LessThan(decimal expect)
        {
            return Matchers.Function((decimal? actual) => actual!=null && expect.SafeCompareTo(actual) > 0, "a decimal < " + expect.ToPrettyString());
        }

        public static IMatcher<decimal?> LessThanOrEqualTo(decimal expect)
        {
            return Matchers.Function((decimal? actual) => actual!=null && expect.SafeCompareTo(actual) >= 0, "a decimal <= " + expect.ToPrettyString());
        }

        public static IMatcher<decimal?> Not(decimal expect)
        {
            return Matchers.Function((decimal? actual) => actual !=null && !Equals(actual.Value,expect), "a decimal != " + expect.ToPrettyString());
        }

        public static IMatcher<decimal?> Between(decimal expectFrom, decimal expectTo)
        {
            return Matchers.Function((decimal? actual) => actual!=null && expectFrom.SafeCompareTo(actual) < 0 && expectTo.SafeCompareTo(actual) > 0, "a decimal where " + expectFrom.ToPrettyString() + " < value < " + expectTo.ToPrettyString());
        }

        public static IMatcher<decimal?> BetweenIncluding(decimal expectFrom, decimal expectTo)
        {
            return Matchers.Function((decimal? actual) => actual!=null && expectFrom.SafeCompareTo(actual) <= 0 && expectTo.SafeCompareTo(actual) >= 0, "a decimal where " + expectFrom.ToPrettyString() + " <= value <= " + expectTo.ToPrettyString());
        }

        public static IMatcher<decimal?> Null()
        {
            return Matchers.Function((decimal? actual) => actual == null, "a null decimal");
        }

        public static IMatcher<decimal?> NotNull()
        {
            return Matchers.Function((decimal? actual) => actual != null, "a non null decimal");
        }
    }

    static class DecimalExtensions
    {
        internal static int SafeCompareTo(this decimal val, decimal? other)
        {
            if (other == null)
            {
                return -1;//treat null as less
            }
            int compare = val.CompareTo(other.Value);
            return compare;
        }
    }
}
