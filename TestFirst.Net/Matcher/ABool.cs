namespace TestFirst.Net.Matcher
{
    public static class ABool
    {
        public static IMatcher<bool?> EqualTo(bool? expect)
        {
            if (expect == null)
            {
                return Null();
            }
            return EqualTo(expect.Value);
        }

        public static IMatcher<bool?> True()
        {
            return EqualTo(true);
        }

        public static IMatcher<bool?> False()
        {
            return EqualTo(false);
        }

        public static IMatcher<bool?> EqualTo(bool expect)
        {
            return Matchers.Function((bool? actual) => actual == expect, "a bool == " + expect);
        }

        public static IMatcher<bool?> Null()
        {
            return Matchers.Function((bool? actual) => actual == null, "a null bool");
        }

        public static IMatcher<bool?> NotNull()
        {
            return Matchers.Function((bool? actual) => actual != null, "a non null bool");
        }
    }
}
