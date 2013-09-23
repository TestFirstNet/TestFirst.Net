using System;

namespace TestFirst.Net.Logging
{
    public class LogLevel:IComparable<LogLevel>
    {
        public static readonly LogLevel Trace = new LogLevel(0,"TRACE");
        public static readonly LogLevel Debug = new LogLevel(1,"DEBUG");
        public static readonly LogLevel Info = new LogLevel(2, "INFO");
        public static readonly LogLevel Warn = new LogLevel(3, "WARN");
        public static readonly LogLevel Error = new LogLevel(4, "ERROR");
        public static readonly LogLevel Off = new LogLevel(5, "OFF");

        private readonly int m_level;
        public String Name { get; private set; }

        private LogLevel(int level, string name)
        {
            m_level = level;
            Name = name;
        }

        public int CompareTo(LogLevel level)
        {
            return m_level - level.m_level;
        }

        /// <summary>
        /// Return the level given the string, never failing. 
        /// </summary>
        /// <returns>On invalid level returns the DEBUG level</returns>
        public static LogLevel SafeParse(String logLevel)
        {
            if (logLevel == null)
            {
                return Debug;
            }
            logLevel = logLevel.Trim().ToUpper();
            switch (logLevel)
            {
                case "TRACE": return Trace;
                case "DEBUG": return Debug;
                case "INFO": return Info;
                case "WARN": return Warn;
                case "ERROR": return Error;
                case "OFF": return Off;
                default: return Debug;
            }
        }
    }
}