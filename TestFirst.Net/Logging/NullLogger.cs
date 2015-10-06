using System;

namespace TestFirst.Net.Logging
{
    /// <summary>
    /// Logger which does nothing
    /// </summary>
    public class NullLogger : ILogger
    {
        public static readonly NullLogger Instance = new NullLogger();

        public bool IsTraceEnabled()
        {
            return false;
        }
        
        public bool IsDebugEnabled()
        {
            return false;
        }

        public bool IsInfoEnabled()
        {
            return false;
        }

        public bool IsWarnEnabled()
        {
            return false;
        }

        public void Trace(string msg)
        {
        }

        public void Trace(string msg, Exception e)
        {
        }

        public void TraceFormat(string msg, params object[] args)
        {
        }

        public void Debug(string msg)
        {
        }

        public void Debug(string msg, Exception e)
        {
        }

        public void DebugFormat(string msg, params object[] args)
        {
        }

        public void Info(string msg)
        {
        }

        public void Info(string msg, Exception e)
        {
        }

        public void InfoFormat(string msg, params object[] args)
        {
        }

        public void Warn(string msg)
        {
        }

        public void Warn(string msg, Exception e)
        {
        }

        public void WarnFormat(string msg, params object[] args)
        {
        }

        public void Error(string msg)
        {
        }

        public void Error(string msg, Exception e)
        {
        }

        public void ErrorFormat(string msg, params object[] args)
        {
        }
    }
}
