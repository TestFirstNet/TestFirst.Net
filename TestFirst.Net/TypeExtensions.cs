using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestFirst.Net
{
    internal static class TypeExtensions
    {
        /// <summary>
        /// Check if the actual type is a subclass of the given type correctly handling 'raw' geenric classes as in IList&lt;,&gt; which strictly are not
        /// superclasses of List&lt;String&gt; but for many type checks it is
        /// </summary>
        /// <param name="superclass">the type of the superclass or interface</param>
        /// <param name="actual"></param>
        /// <returns></returns>
        public static bool IsSuperclassOrInterfaceOf(this Type superclass, Type actual)
        {
            if (superclass.IsAssignableFrom(actual))
            {
                return true;
            }

            if (superclass.IsGenericType && superclass.GetGenericArguments().All(type => type.FullName == null))//eg IList<>, IDictionary<,>
            {
                var rawSuperTypeClass = GetRawTypeName(superclass);

                if (GetRawTypeName(actual) == rawSuperTypeClass)
                {
                    return true;
                }
                if (superclass.IsInterface)
                {
                    foreach (var interfce in actual.GetInterfaces())
                    {
                        if (GetRawTypeName(interfce) == rawSuperTypeClass)
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    for (var parent = actual.BaseType; parent != null; parent = parent.BaseType)
                    {
                        if (GetRawTypeName(parent) == rawSuperTypeClass)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private static String GetRawTypeName(Type t)
        {
            var fullName = t.Namespace + "." + t.Name;//FullName is null on generic types and interfaces when using IList<,>
            var idx = fullName.IndexOf('`');
            if (idx != -1)
            {
                return fullName.Substring(0, idx);
            }
            return fullName;
        }
    }
}
