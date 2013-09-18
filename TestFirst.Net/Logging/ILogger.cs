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

        void Trace(String msg);
        void Trace(String msg, Exception e);
        void TraceFormat(String msg, params object[] args);
                
        void Debug(String msg);
        void Debug(String msg, Exception e);
        void DebugFormat(String msg, params object[] args);

        void Info(String msg);
        void Info(String msg, Exception e);
        void InfoFormat(String msg,params object[] args);

        void Warn(String msg);
        void Warn(String msg, Exception e);
        void WarnFormat(String msg,params object[] args);

        void Error(String msg);
        void Error(String msg, Exception e);
        void ErrorFormat(String msg,params object[] args);
    }
}