namespace TestFirst.Net.Matcher
{
    public static class AnInt
    {

        public static IMatcher<int?> EqualTo(int? expect)
        {
            if (expect == null)
            {
                return Null();
            }
            return EqualTo(expect.Value);
        }

        public static IMatcher<int?> EqualTo(int expect)
        {
            return Matchers.Function((int? actual) => actual.Equals(expect), "an integer == " + expect);
        }

        public static IMatcher<int?> GreaterThan(int expect)
        {
            return Matchers.Function((int? actual) => actual != null && actual > expect, "an integer > " + expect);
        }

        public static IMatcher<int?> GreaterOrEqualTo(int expect)
        {
            return Matchers.Function((int? actual) => actual != null && actual >= expect, "an integer >= " + expect);
        }

        public static IMatcher<int?> LessThan(int expect)
        {
            return Matchers.Function((int? actual) => actual != null && actual < expect, "an integer < " + expect);
        }

        public static IMatcher<int?> LessThanOrEqualTo(int expect)
        {
            return Matchers.Function((int? actual) => actual != null && actual <= expect, "an integer <= " + expect);
        }

        public static IMatcher<int?> Not(int expect)
        {
            return Matchers.Function((int? actual) => actual != null && actual != expect, "an integer != " + expect);
        }

        public static IMatcher<int?> Between(int expectFrom, int expectTo)
        {
            return Matchers.Function((int? actual) => actual != null && actual > expectFrom && actual < expectTo, "an integer where " + expectFrom + " < value < " + expectTo);
        }

        public static IMatcher<int?> BetweenIncluding(int expectFrom, int expectTo)
        {
            return Matchers.Function((int? actual) => actual != null && actual >= expectFrom && actual <= expectTo, "an integer where " + expectFrom + " <= value <= " + expectTo);
        }

        public static IMatcher<int?> Null()
        {
            return Matchers.Function((int? actual) => actual == null, "a null integer");
        }

        public static IMatcher<int?> NotNull()
        {
            return Matchers.Function((int? actual) => actual != null, "a non null integer");
        }
    }
}
