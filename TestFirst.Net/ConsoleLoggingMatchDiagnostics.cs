using System;

namespace TestFirst.Net
{
    /// <summary>
    /// Add an <see cref="Description.IAppendListener"/> which logs everything to the console as it goes. Useful when
    /// integrating with third parties libraries and it is not possible to access the diagnostics after the matching
    /// </summary>
    public class ConsoleLoggingMatchDiagnostics : MatchDiagnostics
    {
        public ConsoleLoggingMatchDiagnostics() 
            : base(new ConsoleAppendListener(null))
        {
        }

        public ConsoleLoggingMatchDiagnostics(string prefix)
            : base(new ConsoleAppendListener(prefix))
        {
        }

        private class ConsoleAppendListener : IAppendListener
        {
            private readonly string m_prefix;
            private bool m_printPrefix = true;

            internal ConsoleAppendListener(string prefix)
            {
                m_prefix = prefix ?? string.Empty;
            }

            public void Append(string text)
            {
                PrintPrefixIfRequired();
                Console.Write(text);
            }

            public void AppendFormat(string text, params object[] args)
            {
                PrintPrefixIfRequired();
                Console.Write(text, args);
            }

            public void AppendLine()
            {
                Console.WriteLine();              
                m_printPrefix = false;
            }

            private void PrintPrefixIfRequired()
            {
                if (m_printPrefix)
                {
                    Console.Write(m_prefix);
                    m_printPrefix = false;
                }
            }
        }
    }
}
