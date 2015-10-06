using System;
using System.Collections.Generic;

namespace TestFirst.Net.Rand
{
    /// <summary>
    /// Wraps registration and retrieval of a value associated with a type, named property on a type, or a propertyType on a type
    /// </summary>
    /// <typeparam name="TValue">the type of the value to associate with a given type, propertyName, or propertyType</typeparam>
    internal class Lookup<TValue>
    {
        private readonly IDictionary<string, TValue> m_values = new Dictionary<string, TValue>();

        public void SetForType(Type propertyType, TValue val)
        {
            Set(Key.ForType(propertyType), val);
        }

        public void SetOnTypeForPropertyName(Type onType, string propertyName, TValue val)
        {
            Set(Key.OnTypeForName(onType, propertyName), val);
        }

        public void SetOnTypeForPropertyType(Type onType, Type propertyType, TValue val)
        {
            Set(Key.OnTypeForType(onType, propertyType), val);
        }

        public void SetAll(Lookup<TValue> lookup)
        {
            foreach (var pair in lookup.m_values)
            {
                Set(pair.Key, pair.Value);
            }
        }

        public bool TryGetValue(Type onType, string propertyName, Type forType, out TValue val)
        {
            // type and name
            if (m_values.TryGetValue(Key.OnTypeForName(onType, propertyName), out val))
            {
                return true;
            }

            // type on type
            if (m_values.TryGetValue(Key.OnTypeForType(onType, forType), out val))
            {
                return true;
            }

            // type
            if (m_values.TryGetValue(Key.ForType(forType), out val))
            {
                return true;
            }

            return false;
        }

        private void Set(string key, TValue val)
        {
            if (m_values.ContainsKey(key))
            {
                m_values.Remove(key);
            }
            m_values.Add(key, val);
        }

        /// <summary>
        /// Creates the keys for various type/property/propertyType combinations. 
        /// </summary>
        private static class Key
        {
            internal static string ForType(Type type)
            {
                return type.FullName;
            }

            internal static string OnTypeForName(Type onType, string name)
            {
                return (onType == null ? "null" : onType.FullName) + ":" + name;
            }

            internal static string OnTypeForType(Type onType, Type ofType)
            {
                return (onType == null ? "null" : onType.FullName) + ":" + ofType.FullName;
            }
        }
    }
}