namespace TestFirst.Net.Matcher
{
    public static class ALong
    {
        public static IMatcher<long?> EqualTo(long? expect)
        {
            if (expect == null)
            {
                return Null();
            }
            return EqualTo(expect.Value);
        }

        public static IMatcher<long?> EqualTo(long expect)
        {
            return Matchers.Function((long? actual) => actual == expect, "a long == " + expect);
        }

        public static IMatcher<long?> Null()
        {
            return Matchers.Function((long? actual) => actual == null, "a null long");
        }

        public static IMatcher<long?> NotNull()
        {
            return Matchers.Function((long? actual) => actual != null, "a non null long");
        }

        public static IMatcher<long?> GreaterThan(long expect)
        {
            return Matchers.Function((long? actual) => actual != null && actual > expect, "a long > " + expect);
        }

        public static IMatcher<long?> GreaterOrEqualTo(long expect)
        {
            return Matchers.Function((long? actual) => actual != null && actual >= expect, "a long >= " + expect);
        }

        public static IMatcher<long?> LessThan(long expect)
        {
            return Matchers.Function((long? actual) => actual != null && actual < expect, "a long < " + expect);
        }

        public static IMatcher<long?> LessThanOrEqualTo(long expect)
        {
            return Matchers.Function((long? actual) => actual != null && actual <= expect, "a long <= " + expect);
        }

        public static IMatcher<long?> Not(long expect)
        {
            return Matchers.Function((long? actual) => actual != null && actual != expect, "a long != " + expect);
        }

        public static IMatcher<long?> Between(long expectFrom, long expectTo)
        {
            return Matchers.Function((long? actual) => actual != null && actual > expectFrom && actual < expectTo, "a long where " + expectFrom + " < value < " + expectTo);
        }

        public static IMatcher<long?> BetweenIncluding(long expectFrom, long expectTo)
        {
            return Matchers.Function((long? actual) => actual != null && actual >= expectFrom && actual <= expectTo, "a long where " + expectFrom + " <= value <= " + expectTo);
        }
    }
}
