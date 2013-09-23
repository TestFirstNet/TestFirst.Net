using System;
using System.Collections;
using System.Reflection;
using System.Text;

namespace TestFirst.Net.Util
{
    public static class ToStringHelper
    {
        private const BindingFlags ReflectionFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.GetField;

        public static string ReflectionToString<T>(T instance)
        {
            var sb = new StringBuilder();
            ReflectionToString(sb, "", instance);
            return sb.ToString();
        }


        private static void ReflectionToString<T>(StringBuilder sb, string offset, T instance)
        {
            if (!typeof(T).IsPrimitive && Equals(instance, default(T)))
            {
                sb.Append("null");
                return;
            }
            sb.Append(instance.GetType().Name);

            var props = instance.GetType().GetProperties(ReflectionFlags);
            foreach(var prop in props)
            {
                if (prop.CanRead)
                {
                    if (prop.GetIndexParameters().Length == 0)
                    {
                        var val = prop.GetValue(instance, null);
                        Append(sb, offset, prop.Name, val);
                    }
                    else
                    {
                        sb.Append(offset).Append("\n\t").Append(prop.Name).Append(":(indexed property ignoring)");
                    }
                }
            }
            var fields = instance.GetType().GetFields(ReflectionFlags);
            foreach(var field in fields)
            {
                var val = field.GetValue(instance);
                Append(sb, offset, field.Name, val);
            }
        }

        
        private static void Append(StringBuilder sb, String offset, String propOrFieldName,Object val )
        {
            sb.Append(offset).Append("\n\t").Append(propOrFieldName).Append(" : ");
            if (val is IList)
            {
                PrintList(sb, offset + "\t", (IList) val);
            }
            else
            {
                sb.Append(val);
            }
        }

        private static void PrintList(StringBuilder sb, string offset, IList list)
        {
            var comma = false;
            foreach (var item in list)
            {
                if (comma)
                {
                    sb.Append(",");
                }
                comma = true;
                ReflectionToString(sb, offset + "\t", item);
            }
        }
    }
}
