using TestFirst.Net.Util;

namespace TestFirst.Net.Matcher
{
    //needs work. Need to add in precision to comparisons!
    public static class AFloat
    {
        public static IMatcher<float?> EqualTo(float? expect)
        {
            if (expect == null)
            {
                return Null();
            }
            return EqualTo(expect.Value);
        }

        public static IMatcher<float?> EqualTo(float expect)
        {
            return Matchers.Function((float? actual) => actual.IsValidFloat() && expect.SafeCompareTo(actual) == 0, "a float == " + expect.ToPrettyString());
        }

        public static IMatcher<float?> Null()
        {
            return Matchers.Function((float? actual) => actual == null, "a null float");
        }

        public static IMatcher<float?> NotNull()
        {
            return Matchers.Function((float? actual) => actual != null, "a non null float");
        }

        public static IMatcher<float?> GreaterThan(float expect)
        {
            return Matchers.Function((float? actual) => actual.IsValidFloat() && expect.SafeCompareTo(actual) < 0, "a float > " + expect.ToPrettyString());
        }

        public static IMatcher<float?> GreaterOrEqualTo(float expect)
        {
            return Matchers.Function((float? actual) => actual.IsValidFloat() && expect.SafeCompareTo(actual) <= 0, "a float >= " + expect.ToPrettyString());
        }

        public static IMatcher<float?> LessThan(float expect)
        {
            return Matchers.Function((float? actual) => actual.IsValidFloat() && expect.SafeCompareTo(actual) > 0, "a float < " + expect.ToPrettyString());
        }

        public static IMatcher<float?> LessThanOrEqualTo(float expect)
        {
            return Matchers.Function((float? actual) => actual.IsValidFloat() && expect.SafeCompareTo(actual) >= 0, "a float <= " + expect.ToPrettyString());
        }

        public static IMatcher<float?> Not(float expect)
        {
            return Matchers.Function((float? actual) => actual.IsValidFloat() && expect.SafeCompareTo(actual) != 0, "a float != " + expect.ToPrettyString());
        }

        public static IMatcher<float?> Between(float expectFrom, float expectTo)
        {
            return Matchers.Function((float? actual) => actual.IsValidFloat() && expectFrom.SafeCompareTo(actual) < 0 && expectTo.SafeCompareTo(actual) > 0, "a float where " + expectFrom.ToPrettyString() + " < value < " + expectTo.ToPrettyString());
        }

        public static IMatcher<float?> BetweenIncluding(float expectFrom, float expectTo)
        {
            return Matchers.Function((float? actual) => actual.IsValidFloat() && expectFrom.SafeCompareTo(actual) <= 0 && expectTo.SafeCompareTo(actual) >= 0, "a float where " + expectFrom.ToPrettyString() + " <= value <= " + expectTo.ToPrettyString());
        }
    }

    static class FloatExtensions
    {
        internal static bool IsValidFloat(this float? val)
        {
            if (val == null)
            {
                return false;
            }
            return !float.IsNaN(val.Value);
        }

        internal static int SafeCompareTo(this float val, float? other)
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
