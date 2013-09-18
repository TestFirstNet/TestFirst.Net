using System;

namespace TestFirst.Net.Logging
{
    public interface ILogFactory
    {
        ILogger GetLogger(Type type);
        ILogger GetLogger(String logName);
    }
}