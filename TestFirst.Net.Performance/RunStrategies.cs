using System;

namespace TestFirst.Net.Performance
{
    public static class RunStrategies
    {
        public static IRunStrategy Fixed(int numRepeats)
        {
            return new FixedNumRunsStrategy(numRepeats);
        }

        public static IRunStrategy Time(TimeSpan time)
        {
            return new TimeBasedRunStrategy(time);
        }

        private class FixedNumRunsStrategy : IRunStrategy
        {
            private readonly int m_numTimesToRun;

            public FixedNumRunsStrategy(int numTimesToRun)
            {
                m_numTimesToRun = numTimesToRun;
            }

            public bool ShouldRun(int runNum)
            {
                return runNum <= m_numTimesToRun;
            }
        }

        private class TimeBasedRunStrategy : IRunStrategy
        {
            private readonly TimeSpan m_time;
            private DateTime? m_firstRunAt;

            public TimeBasedRunStrategy(TimeSpan time)
            {
                m_time = time;
            }

            public bool ShouldRun(int runNum)
            {
                if (m_firstRunAt == null)
                {
                    m_firstRunAt = DateTime.Now;
                    return true;
                }

                return DateTime.Now - m_firstRunAt <= m_time;
            }
        }
    }
}
