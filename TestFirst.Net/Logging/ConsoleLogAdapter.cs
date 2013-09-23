using System;

namespace TestFirst.Net.Logging
{
    public class ConsoleLogAdapter : BaseLogger
    {
        public ConsoleLogAdapter(LogLevel level, string logName):base(level,logName)
        {
        }

        public ConsoleLogAdapter(LogLevel level, Type type):base(level,type)
        {
        }

        protected override void LogLine(LogLevel level, String line)
        {
            Console.WriteLine(line);
        }
    }
}