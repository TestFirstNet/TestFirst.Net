using System;

namespace TestFirst.Net.Matcher.Internal
{
    internal interface IProvidePrettyTypeName
    {
        string GetPrettyTypeName();
    }

    internal static class ProvidePrettyTypeName
    {
        public static string GetPrettyTypeNameFor<TType>()
        {
            var type = typeof(TType);
            bool isNullable = !type.IsClass && (Nullable.GetUnderlyingType(type) != null);
            if (isNullable)
            {
                return Nullable.GetUnderlyingType(type).FullName + "?";
            }
            return type.FullName;
        }
    }
}