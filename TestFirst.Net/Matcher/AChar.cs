using System;

namespace TestFirst.Net.Matcher
{
    public static class AChar
    {
        public static IMatcher<char?> EqualTo(char? expect)
        {
            return Matchers.Function((char? actual) =>
                {
                    if (expect == null && actual == null)
                    {
                        return true;
                    }
                    if (expect == null || actual == null)
                    {
                        return false;
                    }
                    return expect.Value.Equals(actual.Value);
                },
                expect ==null?"a null char":"a char == '" + expect + "'"
             );
        }

        public static IMatcher<char?> NotEqualTo(char? expect)
        {
            return Matchers.Function((char? actual) =>
                {
                    if (expect == null && actual == null)
                    {
                        return false;
                    }
                    if (expect == null || actual == null)
                    {
                        return true;
                    }
                    return !expect.Value.Equals(actual.Value);
                },
                expect==null?"a non null char":"a char != '" + expect + "'"
             );
        }

        public static IMatcher<char?> NotNull()
        {
            return Matchers.Function((char? actual) => actual != null, "a non null char");
        }

        public static IMatcher<char?> Null()
        {
            return Matchers.Function((char? actual) => actual == null, "a null char" );
        }
    }
}
