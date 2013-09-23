using System;
using System.Diagnostics;

namespace TestFirst.Net.Performance
{
    public class PerformanceMetric
    {
        public string Name { get; set; }
        public string CallId { get; set; }
        public double Value { get; set; }        
        public DateTime Timestamp { get; set; }
        public bool IsError { get; set; }
        /// <summary>
        /// Any additional data to be logged. String can contain anything. In the case of errors it is 
        /// recommended to attach the error message
        /// </summary>
        public String Data { get; set; }

        public PerformanceMetric()
        {
            Timestamp = DateTime.Now;
        }

        public static PerformanceMetric NameValue(string name, double value)
        {
            return new PerformanceMetric{ Name=name, Value = value };
        }

        public static PerformanceTimer NewTimerNamed(string name)
        {
            return new PerformanceTimer(name);
        }

        public override String ToString()
        {
            return String.Format("Metric '{0}', CallId '{1}',  Timestamp '{2}', Value '{3}', IsError {4}, Data {5}",Name, CallId, Timestamp.ToString("HHmm:ss.fff"), Value, IsError, Data);
        }

        /// <summary>
        /// Helper class to make collecting call times easier.
        /// </summary>
        public class PerformanceTimer
        {
            private readonly string m_metricName;
            private readonly Stopwatch m_stopWatch = new Stopwatch();
            private int m_callCounter;
            
            public PerformanceTimer(string metricName)
            {
                m_metricName = metricName;
            }

            public PerformanceMetric CollectMetricFor(IPerformanceTestListener testListener, Action action)
            {
                var metric = CollectMetricFor(action);
                testListener.OnMetric(metric);
                return metric;
            }

            /// <summary>
            /// Return a timign metric for the given action. If the action throws an exception the metric returned will be flagged
            /// as an error metric
            /// </summary>
            /// <param name="action"></param>
            /// <returns></returns>
            public PerformanceMetric CollectMetricFor(Action action)
            {
                m_callCounter++;
                try
                {
                    m_stopWatch.Restart();
                    action.Invoke();
                    m_stopWatch.Stop();
                    return new PerformanceMetric{Name = m_metricName, CallId = m_callCounter.ToString(), Value = m_stopWatch.ElapsedMilliseconds, IsError = false};
                }
                catch (Exception e)
                {
                    m_stopWatch.Stop();
                    return new PerformanceMetric{Name = m_metricName, CallId = m_callCounter.ToString(), Value = m_stopWatch.ElapsedMilliseconds, IsError = true, Data = e.Message};
                }
            }
        }
    }
}
