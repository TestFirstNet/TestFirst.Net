using System;
using System.Net;

namespace TestFirst.Net.Examples.Net.Http
{
    internal static class ResponseExtensions
    {
        public static bool Completed(this HttpListenerResponse response)
        {
            try
            {
                return !response.OutputStream.CanWrite;
            }
            catch (ObjectDisposedException)
            {
                return true;
            }
        }
    }
}
