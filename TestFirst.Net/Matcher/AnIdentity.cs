using System;
using System.Security.Principal;

namespace TestFirst.Net.Matcher
{
    public class AnIdentity : PropertyMatcher<IIdentity>
    {
        //for auto complete goodness
        private static readonly IIdentity PropertyNames = null;

        public static AnIdentity With()
        {
            return new AnIdentity();
        }

        public AnIdentity Name(String val)
        {
            Name(AString.EqualTo(val));
            return this;
        }

        public AnIdentity Name(IMatcher<String> matcher)
        {
            WithProperty(() => PropertyNames.Name, matcher);
            return this;
        }

        public AnIdentity IsAuthenticated(bool val)
        {
            WithProperty(() => PropertyNames.IsAuthenticated, ABool.EqualTo(val));
            return this;
        }

        public AnIdentity AuthenticationType(String val)
        {
            WithProperty(() => PropertyNames.AuthenticationType, AString.EqualTo(val));
            return this;
        }
    }
}
