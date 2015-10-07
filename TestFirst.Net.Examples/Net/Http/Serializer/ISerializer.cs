using System;
using System.IO;

namespace TestFirst.Net.Examples.Net.Http.Serializer
{
    internal interface ISerializer
    {
        void Serialize(TextWriter writer, object dto);
        object Deserialize(TextReader reader, Type targetType);
    }
}