namespace TestFirst.Net.Matcher
{
    public static class AnArray
    {

        //TODO:make this work right!
        public static IMatcher<T[]> EqualTo<T>(T[] expect)
        {
            return Matchers.Function((T[] actual) => false, "an array == " + expect);
        }

        public static IMatcher<T[]> Null<T>()
        {
            return Matchers.Function((T[] actual) => actual == null, "a null array");
        }

        public static IMatcher<T[]> NotNull<T>()
        {
            return Matchers.Function((T[] actual) => actual != null, "a non null array");
        }
    }
}
