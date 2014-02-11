using TestFirst.Net.Util;

namespace TestFirst.Net.Matcher
{
    public static class ADouble
    {
        public static IMatcher<double?> EqualTo(double? expect)
        {
            if (expect == null)
            {
                return Null();
            }
            return Matchers.Function((double? actual) => expect.Value.SafeCompareTo(actual) == 0, "a double == " + expect.ToPrettyString());
        }

        public static IMatcher<double?> GreaterThan(double expect)
        {
            return Matchers.Function((double? actual) => actual.IsValidDouble() && expect.SafeCompareTo(actual) < 0, "a double > " + expect.ToPrettyString());
        }

        public static IMatcher<double?> GreaterOrEqualTo(double expect)
        {
            return Matchers.Function((double? actual) => actual.IsValidDouble() && expect.SafeCompareTo(actual) <= 0, "a double >= " + expect.ToPrettyString());
        }

        public static IMatcher<double?> LessThan(double expect)
        {
            return Matchers.Function((double? actual) => actual.IsValidDouble() && expect.SafeCompareTo(actual) > 0, "a double < " + expect.ToPrettyString());
        }

        public static IMatcher<double?> LessThanOrEqualTo(double expect)
        {
            return Matchers.Function((double? actual) => actual.IsValidDouble() && expect.SafeCompareTo(actual) >= 0, "a double <= " + expect.ToPrettyString());
        }

        public static IMatcher<double?> Not(double expect)
        {
            return Matchers.Function((double? actual) => actual.IsValidDouble() && actual != expect, "a double != " + expect.ToPrettyString());
        }

        public static IMatcher<double?> Between(double expectFrom, double expectTo)
        {
            return Matchers.Function((double? actual) => actual.IsValidDouble() && expectFrom.SafeCompareTo(actual) < 0 && expectTo.SafeCompareTo(actual) > 0, "a double where " + expectFrom.ToPrettyString() + " < value < " + expectTo.ToPrettyString());
        }

        public static IMatcher<double?> BetweenIncluding(double expectFrom, double expectTo)
        {
            return Matchers.Function((double? actual) => actual.IsValidDouble() && expectFrom.SafeCompareTo(actual) <= 0 && expectTo.SafeCompareTo(actual) >= 0, "a double where " + expectFrom.ToPrettyString() + " <= value <= " + expectTo.ToPrettyString());
        }

        public static IMatcher<double?> Null()
        {
            return Matchers.Function((double? actual) => actual == null, "a null double");
        }

        public static IMatcher<double?> NotNull()
        {
            return Matchers.Function((double? actual) => actual != null, "a non null double");
        }
    }

    static class DoubleExtensions
    {
        internal static bool IsValidDouble(this double? val)
        {
            if (val == null)
            {
                return false;
            }
            return !double.IsNaN(val.Value);
        }

        internal static int SafeCompareTo(this double val, double? other)
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
