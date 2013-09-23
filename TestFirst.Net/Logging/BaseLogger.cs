using System;
using System.Threading;

namespace TestFirst.Net.Logging
{
    public abstract class BaseLogger : ILogger
    {
        private const string DateTimeFormat = "HHmm:ss.fff";

        private readonly String m_logName;
        private readonly LogLevel m_level;

        public BaseLogger(LogLevel level, Type type) : this(level, type.FullName)
        {}

        public BaseLogger(LogLevel level, string logName)
        {
            m_logName = logName;
            m_level = level;
        }

        public bool IsTraceEnabled()
        {
            return IsLevel(LogLevel.Trace);
        }

        public bool IsDebugEnabled()
        {
            return IsLevel(LogLevel.Debug);
        }

        public bool IsInfoEnabled()
        {
            return IsLevel(LogLevel.Info);
        }

        public bool IsWarnEnabled()
        {
            return IsLevel(LogLevel.Warn);
        }

        public void Trace(String msg)
        {
            Log(LogLevel.Debug, msg);
        }

        public void Trace(String msg, Exception e)
        {
            Log(LogLevel.Debug, msg, e);
        }

        public void TraceFormat(String msg,params object[] args)
        {
            LogFormat(LogLevel.Trace, msg, args);
        }

        public void Debug(String msg)
        {
            Log(LogLevel.Debug, msg);
        }

        public void Debug(String msg, Exception e)
        {
            Log(LogLevel.Debug, msg, e);
        }

        public void DebugFormat(String msg,params object[] args)
        {
            LogFormat(LogLevel.Debug, msg, args);
        }

        public void Info(String msg)
        {
            Log(LogLevel.Info, msg);
        }

        public void Info(String msg, Exception e)
        {           
            Log(LogLevel.Info, msg, e);
        }

        public void InfoFormat(String msg,params object[] args)
        {
            LogFormat(LogLevel.Info, msg, args);
        }

        public void Warn(String msg)
        {
            Log(LogLevel.Warn, msg);
        }

        public void Warn(String msg, Exception e)
        {
            Log(LogLevel.Warn, msg, e);
        }

        public void WarnFormat(String msg,params object[] args)
        {
           LogFormat(LogLevel.Warn, msg, args);
        }

        public void Error(String msg)
        {
            Log(LogLevel.Error, msg);
        }

        public void Error(String msg, Exception e)
        {
            Log(LogLevel.Error, msg, e);
        }

        public void ErrorFormat(String msg,params object[] args)
        {
            LogFormat(LogLevel.Error, msg, args);
        }

        private void Log(LogLevel level, string msg)
        {
            if (IsLevel(level) && IsLoggingEnabled())
            {
                var line = String.Format("[{0}] [{1}] [{2}] [{3}] {4}", 
                    level.Name,                    
                    DateTime.Now.ToString(DateTimeFormat),
                    Thread.CurrentThread.ManagedThreadId, 
                    m_logName,
                    msg);
                LogLine(level, line);
            }
        }

        private void Log(LogLevel level, string msg, Exception e)
        {
            if (IsLevel(level) && IsLoggingEnabled())
            {
                var line = String.Format("[{0}] [{1}] [{2}] [{3}] {4}, cause: {5}", 
                    level.Name,                    
                    DateTime.Now.ToString(DateTimeFormat), 
                    Thread.CurrentThread.ManagedThreadId,
                    m_logName, 
                    msg, 
                    e.StackTrace);
                LogLine(level, line);
            }
        }

        private void LogFormat(LogLevel level, string msg, params object[] args)
        {
            if (IsLevel(level) && IsLoggingEnabled())
            {
                var formattedMsg = String.Format(msg, args);

                var line = String.Format("[{0}] [{1}] [{2}] [{3}] {4}", 
                    level.Name,                    
                    DateTime.Now.ToString(DateTimeFormat), 
                    Thread.CurrentThread.ManagedThreadId,
                    m_logName, 
                    formattedMsg);
                LogLine(level, line);
            }
        }

        private bool IsLevel(LogLevel level)
        {
            return m_level.CompareTo(level) <= 0 ;
        }

        protected abstract void LogLine(LogLevel level, String msg);

        /// <summary>
        /// Subclasses can override to provide custom enabling of logging. Log level is take into account first though
        /// </summary>
        /// <returns></returns>
        protected virtual bool IsLoggingEnabled()
        {
            return true;
        }
    }
}