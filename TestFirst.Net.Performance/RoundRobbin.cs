using System;
using System.Collections.Generic;
using System.Linq;

namespace TestFirst.Net.Performance
{
    /// <summary>
    /// Run the tests in an endless loop
    /// </summary>
    public class RoundRobbin: PerformanceSuite.ITestProvider
    {
        private readonly IPerformanceTest[] m_tests;
        private readonly int m_len;
        private int m_current = -1;

        private readonly Object m_lock = new Object();

        public static Builder With()
        {
            return new Builder();
        }

        private RoundRobbin(IList<IPerformanceTest> tests)
        {
            m_len = tests.Count();
            m_tests = new IPerformanceTest[m_len];
            for (int i = 0; i < m_len; i++)
            {
                m_tests[i] = tests[i];
            }
        }

        public IPerformanceTest Next()
        {
            lock (m_lock)
            {
                m_current++;
                if (m_current >= m_len)
                {
                    m_current = 0;
                }
                return m_tests[m_current];
            }
        }

        public class Builder : IBuilder<PerformanceSuite.ITestProvider>
        {
            private readonly List<IPerformanceTest> m_tests = new List<IPerformanceTest>();

            public Builder Test(IBuilder<IPerformanceTest> builder)
            {
                Test(builder.Build());
                return this;
            }

            public Builder Test(IPerformanceTest test)
            {
                m_tests.Add(test);
                return this;
            }

            public Builder Tests(params IPerformanceTest[] tests)
            {
                m_tests.AddRange(tests);
                return this;
            }

            public Builder Tests(params IBuilder<IPerformanceTest>[] tests)
            {
                m_tests.AddRange(tests.Select(builder=>builder.Build()));
                return this;
            }

            public PerformanceSuite.ITestProvider Build()
            {
                return new RoundRobbin(m_tests);
            }
        }
    }
}