using System;
using System.Net;

namespace TestFirst.Net.Matcher
{
    public class ACookie : PropertyMatcher<Cookie>
    {
        private static readonly Cookie PropertyNames = null;

        public static ACookie With()
        {
            return new ACookie();
        }

        public ACookie Name(string name)
        {
            Name(AString.EqualTo(name));
            return this;
        }

        public ACookie Name(IMatcher<string> matcher)
        {
            WithProperty(() => PropertyNames.Name, matcher);
            return this;
        }

        public ACookie Value(string name)
        {
            Value(AString.EqualTo(name));
            return this;
        }

        public ACookie Value(IMatcher<string> matcher)
        {
            WithProperty(() => PropertyNames.Value, matcher);
            return this;
        }

        public ACookie Path(string name)
        {
            Path(AString.EqualTo(name));
            return this;
        }

        public ACookie Path(IMatcher<string> matcher)
        {
            WithProperty(() => PropertyNames.Path, matcher);
            return this;
        }

        public ACookie Expires(IMatcher<DateTime?> matcher)
        {
            WithProperty(() => PropertyNames.Expires, matcher);
            return this;
        }

        public ACookie TimeStamp(IMatcher<DateTime?> matcher)
        {
            WithProperty(() => PropertyNames.TimeStamp, matcher);
            return this;
        }
        
        public ACookie Secure(bool val)
        {
            WithProperty(() => PropertyNames.Secure, ABool.EqualTo(val));
            return this;
        }
    }
}
