using System;

namespace TestFirst.Net.Lang
{
    public static class PreConditions
    {
        public static T AssertNotNull<T>(T val, string name)
        {
            if (val == null)
            {
                throw new ArgumentException("Expected non null value for argument:" + name);
            }
            return val;
        }

        public static string AssertNotNullOrWhitespace(string val, string name)
        {
            if (val == null)
            {
                throw new ArgumentException("Expected non null value for argument:" + name);
            }
            if (string.IsNullOrWhiteSpace(val))
            {
                throw new ArgumentException("Expected non empty or blank value for argument:" + name);
            }
            return val;
        }

        public static void AssertTrue(bool val, string name, string desc)
        {
            if (!val)
            {
                throw new ArgumentException(string.Format("Expected {0} for argument {1}", desc, name));
            }
        }

        public static void AssertTrue(bool val, string desc)
        {
            if (!val)
            {
                throw new ArgumentException(string.Format("Expected {0}", desc));
            }
        }

        public static bool AssertFalse(bool val, string msg)
        {
            if (val)
            {
                throw new ArgumentException(msg);
            }
            return false;
        }
    }
}
