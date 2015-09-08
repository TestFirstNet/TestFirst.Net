using System.Collections.Generic;
using NUnit.Framework;
using TestFirst.Net.Matcher;

namespace TestFirst.Net.Test.Matcher
{
    [TestFixture]
    public class AKeyValuePairTest:BaseMatcherTest
    {
        [Test]
        public void EqualTo()
        {
            AssertPasses (Pair("key","val"),AKeyValuePair.EqualTo("key","val"));
            AssertPasses (Pair(1,"one"),AKeyValuePair.EqualTo(1,"one"));
            AssertPasses (Pair(1,2),AKeyValuePair.EqualTo(1,2));

            AssertFails (Pair("key","val"),AKeyValuePair.EqualTo("key","not_val"));
            AssertFails (Pair("key","val"),AKeyValuePair.EqualTo("not_key","val"));

            AssertFails (Pair(1,"one"),AKeyValuePair.EqualTo(1,"two"));
            AssertFails (Pair(1,"one"),AKeyValuePair.EqualTo(2,"one"));

            AssertFails (Pair(1,2),AKeyValuePair.EqualTo(1,3));

        }

        [Test]
        public void EqualToWithValueMatcher()
        {
            AssertPasses (Pair("key","val"),AKeyValuePair.EqualTo("key",AString.Containing("val")));
            AssertFails (Pair("key","val"),AKeyValuePair.EqualTo("key",AString.Containing("not_val")));
        }

        [Test]
        public void EqualToWithKeyAndValueMatcher()
        {
            AssertPasses (Pair("key","val"),AKeyValuePair.EqualTo(AString.Containing("key"),AString.Containing("val")));
 
            AssertFails (Pair("key","val"),AKeyValuePair.EqualTo(AString.Containing("key"),AString.Containing("not_val")));
            AssertFails (Pair("key","val"),AKeyValuePair.EqualTo(AString.Containing("not_key"),AString.Containing("val")));
        }


        private KeyValuePair<K,V> Pair<K,V>(K key, V value){
            return new KeyValuePair<K,V> (key, value);
        }
    }
}

