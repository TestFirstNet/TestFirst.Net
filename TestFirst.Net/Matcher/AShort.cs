namespace TestFirst.Net.Matcher
{
    public static class AShort
    {
        public static IMatcher<short?> EqualTo(short? expect)
        {
            if (expect == null)
            {
                return Null();
            }
            return EqualTo(expect.Value);
        }

        public static IMatcher<short?> EqualTo(short expect)
        {
            return Matchers.Function((short? actual) => actual == expect, "a short == " + expect);
        }

        public static IMatcher<short?> GreaterThan(short expect)
        {
            return Matchers.Function((short? actual) => actual != null && actual > expect, "a short > " + expect);
        }

        public static IMatcher<short?> GreaterOrEqualTo(short expect)
        {
            return Matchers.Function((short? actual) => actual != null && actual >= expect, "a short >= " + expect);
        }

        public static IMatcher<short?> LessThan(short expect)
        {
            return Matchers.Function((short? actual) => actual != null && actual < expect, "a short < " + expect);
        }

        public static IMatcher<short?> LessThanOrEqualTo(short expect)
        {
            return Matchers.Function((short? actual) => actual != null && actual <= expect, "a short <= " + expect);
        }

        public static IMatcher<short?> Not(short? expect)
        {
            return Matchers.Function((short? actual) => actual != null && actual != expect, "a short != " + expect);
        }

        public static IMatcher<short?> Between(short? expectFrom, short expectTo)
        {
            return Matchers.Function((short? actual) => actual != null && actual > expectFrom && actual < expectTo, "a short where " + expectFrom + " < value < " + expectTo);
        }

        public static IMatcher<short?> BetweenIncluding(short expectFrom, short expectTo)
        {
            return Matchers.Function((short? actual) => actual != null && actual >= expectFrom && actual <= expectTo, "a short where " + expectFrom + " <= value <= " + expectTo);
        }

        public static IMatcher<short?> Null()
        {
            return Matchers.Function((short? actual) => actual == null, "a null short");
        }

        public static IMatcher<short?> NotNull()
        {
            return Matchers.Function((short? actual) => actual != null, "a non null short");
        }
    }
}
