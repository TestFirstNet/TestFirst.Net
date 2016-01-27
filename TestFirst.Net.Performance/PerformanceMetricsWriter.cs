using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using TestFirst.Net.Lang;
using TestFirst.Net.Logging;

namespace TestFirst.Net.Performance
{
    /// <summary>
    /// Generates a report based from listening to the performance test
    /// </summary>
    [ThreadSafe]
    public class PerformanceMetricsWriter : IPerformanceTestRunnerListener
    {
        private static readonly ILogger Log = Logger.GetLogger<PerformanceMetricsWriter>();

        // try to minimise any delays caused by the file system
        private readonly QueuedFileWriter m_writer = new QueuedFileWriter();

        // ignore any incoming metrics after the test run
        private volatile bool m_collectingMetrics;

        private string m_workingDir = "perf-metrics/";
        private string m_testName;
        private DateTime m_startTime;
        private string m_metricsFilePath;
        
        public static PerformanceMetricsWriter With()
        {
            return new PerformanceMetricsWriter();
        }

        /// <summary>
        /// Override the default location to generate metrics and reports in
        /// </summary>
        /// <param name="dir">The working dir</param>
        /// <returns>The PerformanceMetricsWriter</returns>
        public PerformanceMetricsWriter WorkingDir(string dir)
        {
            m_workingDir = dir;
            return this;
        }
        
        public PerformanceMetricsWriter TestName(object test)
        {
            TestName(test.GetType().Name);
            return this;
        }

        public PerformanceMetricsWriter TestName(string name)
        {
            m_testName = name;
            return this;
        }

        public void OnBeginTestSession()
        {
            m_metricsFilePath = CreateMetricsFile();
            Log.InfoFormat("Collecting metrics, writing to '{0}'", m_metricsFilePath);            
            Console.WriteLine("Writing metrics to " + m_metricsFilePath);

            m_writer.Given_filePath = m_metricsFilePath;

            m_writer.Start();
            
            m_collectingMetrics = true;
        }

        public void OnBeginTestRun()
        {
            m_startTime = DateTime.Now;
        }

        public void OnMetric(TestId testId, PerformanceMetric metric)
        {
            if (m_collectingMetrics)
            {
                m_writer.WriteLine(MetricsFileUtil.ToLine(testId, m_startTime, metric));
            }
        }

        public void OnError(TestId testId, Exception testException)
        {
            Log.Warn("Test Error in : " + testId + ", exception : " + testException);
        }

        public void OnEndTestRun()
        {
            // do nothing
        }

        public void OnEndTestSession()
        {
            m_collectingMetrics = false;
            m_writer.Stop();
        }

        public PerformanceReport BuildReport()
        {
            PreConditions.AssertNotNullOrWhitespace(m_testName, "TestName");
            Log.InfoFormat("Generating report, reading metrics from '{0}'", m_metricsFilePath);

            // TODO:print a running report every X seconds
            var report = new PerformanceReportBuilder
                {
                    ReportTitle = m_testName,
                    MetricsFilePath = m_metricsFilePath,
                    LineToMetricsReader = ReadLineToMetrics
                }
                .Build();

            Log.Debug("Done generating report");

            WriteReportToDisk(report);

            return report;
        }

        private string CreateMetricsFile()
        {
            var path = string.Format(
                "{0}metrics_{1}{2}.csv",
                m_workingDir,
                m_testName == null ? string.Empty : m_testName + "_",
                DateTime.Now.ToString("yyyyMMdd-HHmmss"));
            path = Path.GetFullPath(path);

            Directory.CreateDirectory(m_workingDir);

            using (File.CreateText(path))
            {
                if (!File.Exists(path))
                {
                    throw new InvalidOperationException(string.Format("Couldn't create the metrics file path: {0}", path));
                }
            }

            using (var writer = File.AppendText(path))
            {
                writer.WriteLine(MetricsFileUtil.Header());
            }

            return path;
        }

        private PerformanceMetric ReadLineToMetrics(string metricLineEntry)
        {
            if (metricLineEntry.StartsWith("#"))
            {
                return null;
            }
            PerformanceMetric metric;
            if (MetricsFileUtil.TryRead(metricLineEntry, out metric))
            {
                return metric;
            }

            return null;
        }

        private void WriteReportToDisk(PerformanceReport report)
        {
            var reportFile = m_metricsFilePath + ".report." + report.GeneratedAt.ToString("yyyy-MM-dd_HHmm.ss") + ".txt";
            using (var writer = File.CreateText(reportFile))
            {
                writer.WriteLine(report.ToString());
            }

            Log.InfoFormat("Printed report to '{0}'", reportFile);
        }

        private static class MetricsFileUtil
        {
            private const string DateFormat = "yyyyMMdd-HHmm:ss.fff";

            private static readonly char[] SplitChars = { ',' };

            private enum Col
            {
                MachineId = 0,
                AgentId = 1,
                ThreadId = 2,
                TimeFromStartMs = 3,
                MetricCallId = 4,
                MetricName = 5,
                MetricTImeStamp = 6,
                MetricValue = 7,
                MetricIsError = 8,
                MetricData = 9
            }

            internal static string Header()
            {
                return "# MachineId, AgentId, ThreadId, TimeFromStartMs, Metric.CallId, Metric.Name, Metric.Timestamp, Metric.Value, Metric.IsError, Metric.Data";
            }

            internal static string ToLine(TestId testId, DateTime testRunStartTime, PerformanceMetric metric)
            {
                return string.Format(
                    "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}",
                    testId.MachineId, 
                    testId.AgentId, 
                    testId.ThreadId,
                    (metric.Timestamp - testRunStartTime).TotalMilliseconds,
                    metric.CallId ?? string.Empty, 
                    metric.Name,  
                    metric.Timestamp.ToString(DateFormat),                                         
                    metric.Value, 
                    metric.IsError ? "t" : "f",
                    metric.Data ?? string.Empty);
            }

            internal static bool TryRead(string line, out PerformanceMetric metric)
            {
                if (!line.StartsWith("#"))
                {
                    var parts = line.Split(SplitChars, (int)Col.MetricData + 1); // leave the Data bit as it is, including seperator chars
                    if (parts.Length >= (int)Col.MetricData - 1)
                    {
                        // testId = new TestId(
                        //    machineId:parts[0],
                        //    agentId:parts[1],
                        //    threadId:parts[2]
                        // );
                        metric = new PerformanceMetric
                        {
                            CallId = NullIfEmpty(parts[(int)Col.MetricCallId]),
                            Name = parts[(int)Col.MetricName],
                            Timestamp = DateTime.ParseExact(parts[(int)Col.MetricTImeStamp], DateFormat, null),
                            Value = double.Parse(parts[(int)Col.MetricValue]),
                            IsError = StringToBool(parts[(int)Col.MetricIsError]),
                            Data = NullIfEmpty(parts[(int)Col.MetricData])
                        };
                        return true;
                    }
                }

                // testId = default(TestId);
                metric = default(PerformanceMetric);
                return false;
            }

            private static string NullIfEmpty(string s)
            {
                return s.Length == 0 ? null : s;
            }

            private static bool StringToBool(string s)
            {
                if (string.IsNullOrEmpty(s))
                {
                    return false;
                }
                return s.Equals("t");
            }
        }

        /// <summary>
        /// Maintains a queues of strings to write to file. reads lines from many threads and writes in a single background thread
        /// </summary>
        [ThreadSafe]
        private class QueuedFileWriter
        {
            private const int WritePollIntervalMs = 300;

            private readonly ConcurrentQueue<string> m_queue = new ConcurrentQueue<string>();
            private volatile string m_pathToFile;

            private bool m_running;
            private bool m_shouldRun;
            private Thread m_writerThread;

            public string Given_filePath
            {
                get { return m_pathToFile; }
                set { m_pathToFile = value; }
            }

            public void Start()
            {
                PreConditions.AssertNotNullOrWhitespace(m_pathToFile, "MetricFilePath");

                ClearQueue();
                StartWriteTask();
            }

            public void WriteLine(string line)
            {
                m_queue.Enqueue(line);
            }

            public void Stop()
            {
                StopWriteTask();
            }

            private void ClearQueue()
            {
                string item;
                while (m_queue.TryDequeue(out item))
                {
                    // do nothing, queue is emptying
                }
            }

            private void StartWriteTask()
            {
                if (!m_running)
                {
                    m_shouldRun = true;
                    m_writerThread = new Thread(() =>
                    {
                        using (var writer = File.AppendText(m_pathToFile))
                        {
                            while (m_shouldRun)
                            {
                                WriteItemsTo(writer);
                                if (m_shouldRun)
                                {
                                    Thread.Sleep(WritePollIntervalMs);
                                }
                            }
                        }
                    });
                    m_writerThread.Start();
                    m_running = true;
                }
            }

            private void StopWriteTask()
            {
                if (m_running)
                {
                    // kill the polling thread.
                    m_shouldRun = false;
                    m_writerThread.Join(TimeSpan.FromSeconds(30));

                    // complete the last poll to clear the queue
                    using (var writer = File.AppendText(m_pathToFile))
                    {
                        WriteItemsTo(writer);
                    }
                    m_running = false;
                }
            }

            private void WriteItemsTo(StreamWriter writer)
            {
                var linesToWrite = TakeItemsFromQueue();
                foreach (var line in linesToWrite)
                {
                    writer.WriteLine(line);
                }
                writer.Flush();
            }

            private IEnumerable<string> TakeItemsFromQueue()
            {
                string line;
                while (m_queue.TryDequeue(out line))
                {
                    yield return line;
                }
            }
        }
    }
}