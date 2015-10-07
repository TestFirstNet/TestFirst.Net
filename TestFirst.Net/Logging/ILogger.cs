using System;

namespace TestFirst.Net.Logging
{
    /// <summary>
    /// Use our own logging interface to allow for a dependency free api. Adapters can simply be used
    /// </summary>
    public interface ILogger
    {
        bool IsTraceEnabled();
        bool IsDebugEnabled();
        bool IsInfoEnabled();
        bool IsWarnEnabled();

        void Trace(string msg);
        void Trace(string msg, Exception e);
        void TraceFormat(string msg, params object[] args);
                
        void Debug(string msg);
        void Debug(string msg, Exception e);
        void DebugFormat(string msg, params object[] args);

        void Info(string msg);
        void Info(string msg, Exception e);
        void InfoFormat(string msg, params object[] args);

        void Warn(string msg);
        void Warn(string msg, Exception e);
        void WarnFormat(string msg, params object[] args);

        void Error(string msg);
        void Error(string msg, Exception e);
        void ErrorFormat(string msg, params object[] args);
    }
}