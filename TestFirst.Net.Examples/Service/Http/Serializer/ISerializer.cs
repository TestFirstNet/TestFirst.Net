using System;
using System.IO;

namespace TestFirst.Net.Examples.Service.Http
{
    internal interface ISerializer
    {
        void Serialize(TextWriter writer, Object dto);
        Object Deserialize(TextReader reader, Type targetType);
    }
}