using System;

namespace TestFirst.Net.Logging
{
    public class SystemDiagnosticsLogAdapter : BaseLogger
    {
        private readonly string m_logName;

        private SystemDiagnosticsLogAdapter(LogLevel level, string name) 
            : base(level, name)
        {
            m_logName = name;
        }

        protected override void LogLine(LogLevel level, string line)
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