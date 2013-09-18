using System;

namespace TestFirst.Net.Logging
{
    public class SystemDiagnosticsLogAdapter : BaseLogger
    {
        private readonly String m_logName;

        private SystemDiagnosticsLogAdapter(LogLevel level, String name) : base(level, name)
        {
            m_logName = name;
        }

        protected override void LogLine(LogLevel level, String line)
        {
            if (level.Equals(LogLevel.Trace))
            {
                System.Diagnostics.Trace.WriteLine(line, m_logName);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine(line, m_logName);
            }            
        }
    }
}