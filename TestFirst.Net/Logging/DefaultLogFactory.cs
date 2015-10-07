using System;

namespace TestFirst.Net.Logging
{
    internal class DefaultLogFactory : ILogFactory
    {
        private readonly LogLevel m_level;

        internal DefaultLogFactory(LogLevel level)
        {
            m_level = level;
        }

        ILogger ILogFactory.GetLogger(Type type)
        {
            return Logger.GetLogger(type.FullName);
        }

        ILogger ILogFactory.GetLogger(string logName)
        {
            return new ConsoleLogAdapter(m_level, logName);
        }
    }
}