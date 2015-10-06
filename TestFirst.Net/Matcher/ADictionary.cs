using System.Collections.Generic;

namespace TestFirst.Net.Matcher
{
    // TODO:convert into a generic dictionary! and take in key/value matchers and save them as a list rather than a direct map
    public class ADictionary : AbstractMatcher<IDictionary<string, string>>
    {
        private readonly Dictionary<string, IMatcher<string>> m_expectedKeyValues = new Dictionary<string, IMatcher<string>>();
 
        public static ADictionary With()
        {
            return new ADictionary();
        }

        public ADictionary KeyMatching(string key, IMatcher<string> value)
        {
            m_expectedKeyValues.Add(key, value);
            return this;
        }

        public ADictionary KeyMatching(string key, string value)
        {
            return KeyMatching(key, AString.EqualTo(value));
        }

        public override bool Matches(IDictionary<string, string> actual, IMatchDiagnostics diag)
        {
            bool matches = true;
            foreach (var key in m_expectedKeyValues)
            {
                string actualValue;
                if (actual.TryGetValue(key.Key, out actualValue))
                    matches &= diag.TryMatch(actualValue, "Value for " + key.Key, key.Value);
                else
                    matches &= diag.TryMatch(null, "Value for " + key.Key, key.Value);
            }
            return matches;
        }
    }
}