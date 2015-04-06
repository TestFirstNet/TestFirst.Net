using System;
using System.IO;
using Newtonsoft.Json;

namespace TestFirst.Net.Examples.Service.Http
{
    public class JsonNetSerializer : ISerializer
    {
        private readonly JsonSerializer m_serializer = new JsonSerializer();

        public void Serialize(TextWriter writer, object dto)
        {
            m_serializer.Serialize(writer, dto);
        }

        public object Deserialize(TextReader reader, Type targetType)
        {
            return m_serializer.Deserialize(reader, targetType);
        }
    }
}