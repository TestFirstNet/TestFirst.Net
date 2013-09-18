using System;
using System.Collections.Generic;
using System.Text;

namespace TestFirst.Net.Matcher
{
    /// <summary>
    /// Provides the basic string matchers
    /// </summary>
    public static class AString
    {
        public static IMatcher<string> Any()
        {
            return AnInstance.Any<String>();
        }
        
        public static IMatcher<string> Not(IMatcher<string> matcher)
        {
            return Matchers.Not(matcher);
        }

        //TODO:convert to use a generic type converter? Put into CoreMatchers?
        /// <summary>
        /// Attempt to parse the string to an int and apply the given int matcher
        /// </summary>
        public static IMatcher<string> As(IMatcher<int?> intMatcher)
        {
            return Matchers.Function((string actual, IMatchDiagnostics diagnostics) =>
            {
                int intActual;
                if (int.TryParse(actual, out intActual))
                {
                    return intMatcher.Matches(intActual, diagnostics);
                }
                else
                {
                    diagnostics.MisMatched("Couldn't parse the string '{0}' as an int", actual);
                }
                return false;
            }, 
            "string of int matching " + intMatcher);
        }
        
        public static IList<IMatcher<string>> EqualToValues(params string[] expects)
        {
            var matchers = new List<IMatcher<string>>(expects.Length);
            foreach (string expect in expects)
            {
                matchers.Add(EqualTo(expect));
            }
            return matchers;
        }
   
        public static IMatcher<string> Null()
        {
            return EqualTo((string)null);
        }

        public static IMatcher<string> NotNull()
        {
            return Matchers.Function((string actual) =>actual != null,"a non null string");
        }
        
        /// <summary>
        /// A string which is either null, empty, or only contains whitespace
        /// </summary>
        public static IMatcher<string> Blank()
        {
            return Matchers.Function((string actual) => string.IsNullOrEmpty(actual) || actual.Trim().Length == 0, "a blank string");
        }

        /// <summary>
        /// A string which is NOT null, empty, or only contains whitespace
        /// </summary>
        public static IMatcher<string> NotBlank()
        {
            return Matchers.Function((string actual) => !string.IsNullOrEmpty(actual) && actual.Trim().Length > 0, "a non blank string");
        }

        public static IMatcher<string> EqualTo(string expect)
        {
            return Matchers.Function((string actual) =>
                {
                    if(expect==null && actual == null)
                    {
                        return true;
                    } 
                    if( expect != null )
                    {
                        return expect.Equals(actual);
                    }
                    return false;
                }, 
                "the string '" + expect + "'"
             );
        }

        public static IMatcher<string> EqualToIgnoringCase(string expect)
        {
            expect = expect.ToLower();
            return Matchers.Function((string actual) => actual != null && expect.Equals(actual.ToLower()), "a string, ignoring case, equal to '" + expect + "'");
        }

        public static IMatcher<string> Containing(string expect)
        {
            return Matchers.Function((string actual) => actual != null && actual.Contains(expect), "a string containing '" + expect + "'");
        } 

        public static IMatcher<string> TrimmedLength(IMatcher<int?> intMatcher)
        {
            return Matchers.Function((string actual, IMatchDiagnostics diagnostics) =>
            {
                actual = actual==null?"":actual.Trim();
                return intMatcher.Matches(actual.Length, diagnostics);
            }, 
            "string length " + intMatcher);
        }

        public static IMatcher<string> ContainingIgnorePunctuationAndCase(string expect)
        {
            expect = RemovePunctuation(expect).ToLower();
            return Matchers.Function((string actual) => actual != null && RemovePunctuation(actual).ToLower().Contains(expect), "a string containing, ignoring case and punctuation, equal to '" + expect + "'");
     
        }

        private static String RemovePunctuation(String s)
        {
            var sb = new StringBuilder();
            foreach (var c in s)
            {
                if (Char.IsLetterOrDigit(c))
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public static IMatcher<string> ContainingOfAnyCase(string expect)
        {
            expect = expect.ToLower();
            return Matchers.Function((string actual) => actual != null && actual.ToLower().Contains(expect), "a string containing, ignoring case, equal to '" + expect + "'");
        }

        public static IMatcher<string> EndingWith(string expect)
        {
            return Matchers.Function((string actual) => actual != null && actual.EndsWith(expect), "a string ending with '" + expect + "'");
        }

        public static IMatcher<string> StartingWith(string expect)
        {
            return Matchers.Function((string actual) => actual != null && actual.StartsWith(expect), "a string starting with '" + expect + "'");
        } 
    }
}
