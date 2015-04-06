using System;

namespace TestFirst.Net.Matcher
{
    public static class AGuid
    {
        public static IMatcher<Guid?> EqualTo(Guid? expect)
        {
            return Matchers.Function((Guid? actual) =>
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
                expect ==null?"a null Guid":"a Guid == '" + expect + "'"
             );
        }

        public static IMatcher<Guid?> NotEqualTo(Guid? expect)
        {
            return Matchers.Function((Guid? actual) =>
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
                expect==null?"a non null Guid":"a Guid != '" + expect + "'"
             );
        }

        public static IMatcher<Guid?> NotEmpty()
        {
            return Matchers.Function((Guid? actual) =>
                {
                    return actual != null && !Guid.Empty.Equals(actual);
                },
                "a non empty or null Guid"
             );
        }

        public static IMatcher<Guid?> NotNull()
        {
            return Matchers.Function((Guid? actual) => actual != null, "a non null Guid");
        }

        public static IMatcher<Guid?> Null()
        {
            return Matchers.Function((Guid? actual) => actual == null, "a null Guid" );
        }
    }
}
