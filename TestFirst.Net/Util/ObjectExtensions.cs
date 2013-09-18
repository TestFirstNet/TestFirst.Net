using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace TestFirst.Net.Util
{
    internal static class ObjectExtensions
    {
        internal static string ToPrettyString(this object val)
        {
            if (val == null)
            {
                return null;
            }
            if (val.GetType().IsPrimitive)
            {
                return val.ToPrettyString(null);
            }
            return val.ToPrettyString(new List<object>());
        }

        internal static string ToPrettyString(this object val, IList<object> alreadyConverted)
        {
            if (val == null)
            {
                return null;
            }
            if (val is float)
            {
                return ((float)val).ToString("R") + "F";
            }
            if (val is double)
            {
                return ((double)val).ToString("N17") + "D";
            }
            if (val is decimal)
            {
                return ((decimal)val).ToString("N29") + "M";
            }
            if (val is DateTime)
            {
                return ((DateTime) val).ToString("yyyy/MM/dd HH:mm:ss.fff");
            }
            if (!val.GetType().IsPrimitive && !(val is String))
            {
                if (alreadyConverted.Contains(val))
                {
                    return String.Format("<!Circular-Reference:{0}@{1}!>", val.GetType().FullName, val.GetHashCode());
                }
                alreadyConverted.Add(val);
            }

            if (val is ICollection)
            {
                var items = val as ICollection;
                return PrettyPrint(items, "List", items.Count, alreadyConverted);
            }

            if( val is IEnumerable)
            {
                var type = val.GetType();
                if (type.Namespace == (typeof(HashSet<>)).Namespace && type.Name.StartsWith("HashSet"))
                {
                    var items = val as dynamic;
                    return PrettyPrint(items, "HashSet", items.Count, alreadyConverted);
                }

                if (type.Namespace == (typeof(SortedSet<>)).Namespace && type.Name.StartsWith("SortedSet"))
                {
                    var items = val as dynamic ;
                    return PrettyPrint(items, "SortedSet", items.Count, alreadyConverted);
                }
            }
            if (val is Cookie)
            {
                var cookie = val as Cookie;
                return ToStringHelper.ReflectionToString(cookie);
            }

            return val.ToString();
        }


        private static string PrettyPrint(IEnumerable items, String prefix, int count, IList<object> alreadyConverted)
        {
            var sb = new StringBuilder();

            bool primitiveItems = IsEnumerationTypeWithPrimitiveElements(items.GetType());
            bool stringItems = IsEnumerationTypeWithStringElements(items.GetType());
            sb.Append("A ").Append(prefix).Append("<");

            if (count >= 0)
            {
                sb.Append("count:").Append(count);               
            }
            sb.Append(">");
            if (count == 0)
            {
                //nothing
            }
            else if (primitiveItems)
            {
                sb.Append("[");
                bool first = true;
                foreach (var item in items)
                {
                    if (!first)
                    {
                        sb.Append(",");
                    }
                    first = false;
                    sb.Append(item.ToPrettyString(alreadyConverted));
                }
                sb.Append("]");
            }
            else if (stringItems)
            {
                sb.Append("[\n\t");
                bool first = true;
                foreach (var item in items)
                {
                    if (!first)
                    {
                        sb.Append(",\n\t");
                    }
                    first = false;
                    sb.Append("'");
                    sb.Append(item.ToPrettyString(alreadyConverted));
                    sb.Append("'");
                }
                sb.Append("\n]");
            }
            else
            {
                sb.Append("[\n");
                bool first = true;
                foreach (var item in items)
                {
                    if (!first)
                    {
                        sb.Append("\n,");
                    }
                    first = false;
                    sb.Append(item.ToPrettyString(alreadyConverted));
                }
                sb.Append("\n]");
            }

            return sb.ToString();
        }

        internal static bool IsEnumerationTypeWithPrimitiveElements(Type listType)
        {
            bool isPrimitiveItems = false;
            if(listType.IsArray)
            {
                isPrimitiveItems = listType.GetElementType().IsPrimitive;
            }
            else if (typeof(ICollection).IsAssignableFrom((listType)) && listType.IsGenericType)
            {
                isPrimitiveItems = listType.GetGenericArguments()[0].IsPrimitive;
            }
            return isPrimitiveItems;
        }

        internal static bool IsEnumerationTypeWithStringElements(Type listType)
        {
            bool isStringItems = false;
            if(listType.IsArray)
            {
                isStringItems = typeof(string).IsAssignableFrom(listType.GetElementType());
            }
            else if (typeof(ICollection).IsAssignableFrom((listType)) && listType.IsGenericType)
            {
                isStringItems = typeof(string).IsAssignableFrom(listType.GetGenericArguments()[0]);
            }
            return isStringItems;
        }
    }
}