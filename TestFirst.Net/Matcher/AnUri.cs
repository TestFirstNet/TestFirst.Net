using System;

namespace TestFirst.Net.Matcher
{
    public static class AnUri
    {
        public static IMatcher<Uri> EqualTo(String fullPath)
        {
            return EqualTo(AString.EqualTo(fullPath));
        }

        public static IMatcher<Uri> EqualTo(IMatcher<String> fullPathMatcher)
        {
            return Matchers.Function((Uri actual,IMatchDiagnostics diag) => diag.TryMatch(actual.AbsoluteUri,fullPathMatcher), ()=>"a uri equal to " + fullPathMatcher);
        } 
        
        public static IMatcher<Uri> EqualTo(Uri expect)
        {
            return Matchers.Function((Uri actual,IMatchDiagnostics diag) => diag.TryMatch(actual,AnInstance.EqualTo(expect)), ()=>"a uri equal to " + expect);
        } 

        public static IMatcher<Uri> Null()
        {
            return Matchers.Function((Uri actual) =>actual == null,"a null uri");
        }

        public static IMatcher<Uri> NotNull()
        {
            return Matchers.Function((Uri actual) =>actual != null,"a non null uri");
        }
    }
}
