using System.Collections.Generic;

namespace TestFirst.Net.Matcher
{
    public static class AKeyValuePair{

        public static IMatcher<KeyValuePair<K,V>> EqualTo<K,V>(K key, V value){
            return EqualTo (AnInstance.EqualTo(key), AnInstance.EqualTo(value));
        }

        public static IMatcher<KeyValuePair<K,V>> EqualTo<K,V>(K key, IMatcher<V> valueMatcher){
            return EqualTo (AnInstance.EqualTo(key), valueMatcher);
        }

        public static IMatcher<KeyValuePair<K,V>> EqualTo<K,V>(IMatcher<K> keyMatcher, IMatcher<V> valueMatcher){
            return Matchers.Function ((KeyValuePair<K,V> actual,IMatchDiagnostics diag)=>{
                return diag.TryMatch(actual.Key,keyMatcher) && diag.TryMatch(actual.Value,valueMatcher);
            },
            desc=>{
                desc.Text("KeyValuePair");
                desc.Value("Key", keyMatcher);
                desc.Text("Value", valueMatcher);
            });
        }
    }
}

