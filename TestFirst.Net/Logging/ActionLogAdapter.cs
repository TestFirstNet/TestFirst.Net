using System;
using TestFirst.Net.Lang;

namespace TestFirst.Net.Logging
{
    /// <summary>
    /// Logger which sends log messages and levels to a passed in action
    /// </summary>
    public class ActionLogAdapter : BaseLogger
    {
        private readonly Action<LogLevel,String> m_onLogLine;

        public ActionLogAdapter(LogLevel level, string logName, Action<LogLevel,String> onLogLine) : base(level, logName)
        {
            PreConditions.AssertNotNull(onLogLine, "logLineAction");
            m_onLogLine = onLogLine;
        }

        public ActionLogAdapter(LogLevel level, Type type, Action<LogLevel,String> onLogLine) : base(level, type)
        {
            PreConditions.AssertNotNull(onLogLine, "logLineAction");
            m_onLogLine = onLogLine;
        }

        protected override void LogLine(LogLevel level, string msg)
        {
            m_onLogLine.Invoke(level, msg);
        }
    }
}
