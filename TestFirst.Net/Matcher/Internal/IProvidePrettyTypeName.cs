using System;

namespace TestFirst.Net.Matcher.Internal
{
    internal interface IProvidePrettyTypeName
    {
        String GetPrettyTypeName();
    }

    internal static class ProvidePrettyTypeName
    {
        public static String GetPrettyTypeNameFor<TType>()
        {
            var type = typeof (TType);
            bool isNullable = !type.IsClass && (Nullable.GetUnderlyingType(type) != null);
            if (isNullable)
            {
                return Nullable.GetUnderlyingType(type).FullName + "?";
            }
            return type.FullName;
        }
    }
}