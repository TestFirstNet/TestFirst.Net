using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TestFirst.Net.Lang;
using TestFirst.Net.Logging;

namespace TestFirst.Net.Performance
{
    public class PerformanceReportBuilder
    {
        private static readonly ILogger Log = Logger.GetLogger<PerformanceReportBuilder>();

        public string ReportTitle { get; set; }
        public string MetricsFilePath { get; set; }

        /// <summary>
        /// Gets or sets the amount of time to ignore from the first sample to allow for a warmup period
        /// </summary>
        public TimeSpan? IgnoreWarmUpPeriodOf { get; set; }

        /// <summary>
        /// Gets or sets the function to convert a line of text to a metric
        /// </summary>
        public Func<string, PerformanceMetric> LineToMetricsReader { get; set; }

        public PerformanceReport Build()
        {
            PreConditions.AssertNotNullOrWhitespace(ReportTitle, "ReportTitle");
            PreConditions.AssertNotNullOrWhitespace(MetricsFilePath, "MetricsPath");
            PreConditions.AssertNotNull(LineToMetricsReader, "MetricsReader");

            var metrics = ReadMetricsFrom(MetricsFilePath);
            metrics = RemoveWarmupEntries(metrics);
            var summaries = CalculateSummaries(metrics);

            var report = new PerformanceReport { Title = ReportTitle, MetricsFilePath = MetricsFilePath };
            report.AddSummaries(summaries);

            return report;
        }

        private IEnumerable<PerformanceMetric> RemoveWarmupEntries(IEnumerable<PerformanceMetric> metrics)
        {
            if (IgnoreWarmUpPeriodOf != null && IgnoreWarmUpPeriodOf.Value.Milliseconds > 0)
            {
                var firstMetric = metrics.FirstOrDefault();
                if (firstMetric != null)
                {
                    var includeAnythingAfter = firstMetric.Timestamp + IgnoreWarmUpPeriodOf.Value;
                    return metrics.Where(m => m.Timestamp > includeAnythingAfter);
                }
            }
            return metrics;
        }

        private IEnumerable<PerformanceMetric> ReadMetricsFrom(string path)
        {
            if (!File.Exists(path))
            {
                // no metrics
                return new List<PerformanceMetric>();
            }

            return ReadMetricsFileFrom(path);
        }

        private IEnumerable<PerformanceMetric> ReadMetricsFileFrom(string path)
        {
            using (var reader = new StreamReader(path))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var metric = LineToMetricsReader.Invoke(line);
                    if (metric != null)
                    {
                        yield return metric;
                    }
                }
            }
        }

        // ReSharper disable PossibleMultipleEnumeration
        private IEnumerable<PerformanceReport.MetricSummary> CalculateSummaries(IEnumerable<PerformanceMetric> metrics)
        {
            var runningCalcsByMetricName = new Dictionary<string, RunningCalculation>();

            // stream all the metrics and aggregate results
            Log.Debug("Calculating averages...");
            var processed = 0;

            // extract all the metric names and perform a first pass
            foreach (var metric in metrics)
            {
                processed++;

                if (!runningCalcsByMetricName.ContainsKey(metric.Name))
                {
                    runningCalcsByMetricName.Add(metric.Name, new RunningCalculation(metric.Name));
                }
                runningCalcsByMetricName[metric.Name].FirstPass(metric);
  
                if (processed % 50 == 0)
                {
                    Log.DebugFormat("Processed #{0}", processed);
                }
            }

            // calculate std deviation now we have the mean average
            Log.Debug("Collecting sum of differences for std deviation...");
            processed = 0;
            foreach (var metric in metrics)
            {
                processed++;

                runningCalcsByMetricName[metric.Name].SecondPass(metric);

                if (processed % 50 == 0)
                {
                    Log.DebugFormat("Processed #{0}", processed);
                }
            }

            // calculate median and throughput
            foreach (var metricName in runningCalcsByMetricName.Keys)
            {
                var name = metricName;
                var metricsWithThisName = metrics.Where(m => m.Name == name);
                runningCalcsByMetricName[metricName].CalcMedian(metricsWithThisName);
                runningCalcsByMetricName[metricName].CalcThroughput(metricsWithThisName);
            }

            // generate summaries from running calcs
            Log.Debug("Calculating summaries...");

            return runningCalcsByMetricName.Values.Select(runningCalc => runningCalc.ToSummary()).ToList();
        }

        // ReSharper restore PossibleMultipleEnumeration
        private class RunningCalculation
        {
            private readonly string m_metricName;

            private int m_validCount;
            private int m_errorCount;
            private double m_min = double.MaxValue;
            private double m_max = double.MinValue;
            private double m_mean;
            private double m_median;
            private double m_thoughPutPerSecond;

            // calculated once we have the mean after the first pass
            private double m_sumOfDiffToTheMeanSqrd;

            public RunningCalculation(string metricName)
            {
                m_metricName = metricName;
            }

            internal void FirstPass(PerformanceMetric metric) // to calculate average
            {
                if (metric.IsError)
                {
                    m_errorCount++;
                    return;
                }
               
                m_validCount++;

                if (m_min > metric.Value)
                {
                    m_min = metric.Value;
                }
                if (m_max < metric.Value)
                {
                    m_max = metric.Value;
                }

                // loss a bit of precision but it prevents overflow if a huge number of metrics
                m_mean += (metric.Value - m_mean) / m_validCount;
            }

            internal void SecondPass(PerformanceMetric metric) // to calculate std deviation based on calculated average
            {
                if (metric.IsError)
                {
                    return;
                }
                var diffToTheMean = metric.Value - m_mean;
                m_sumOfDiffToTheMeanSqrd += diffToTheMean * diffToTheMean;
            }

            internal void CalcMedian(IEnumerable<PerformanceMetric> metrics) // to calculate std deviation based on calculated average
            {
                var sorted = metrics.Where(m => !m.IsError).ToList();
                if (sorted.Count > 0)
                {
                    sorted.Sort((left, right) => left.Value.CompareTo(right.Value));
                    m_median = sorted[sorted.Count / 2].Value;
                }
            }

            internal void CalcThroughput(IEnumerable<PerformanceMetric> metrics) // to calculate std deviation based on calculated average
            {
                var sorted = metrics.Where(m => !m.IsError).ToList();
                sorted.Sort((left, right) => left.Timestamp.CompareTo(right.Timestamp));

                if (sorted.Count > 1)
                {
                    var start = sorted[0].Timestamp;
                    var end = sorted[sorted.Count - 1].Timestamp;
                    var timeRangeSecs = (end - start).TotalSeconds;

                    m_thoughPutPerSecond = sorted.Count / timeRangeSecs;
                }
            }

            internal PerformanceReport.MetricSummary ToSummary()
            {
                var summary = new PerformanceReport.MetricSummary();
                summary.MetricName = m_metricName;
                summary.TotalCount = m_validCount + m_errorCount;
                summary.ErrorCount = m_errorCount;
                summary.ValueMean = m_mean;
                summary.ValueStdDeviation = Math.Sqrt(m_sumOfDiffToTheMeanSqrd / m_validCount);
                summary.ValueMin = m_min;
                summary.ValueMax = m_max;
                summary.ValueMedian = m_median;
                summary.ThroughPutPerSecond = m_thoughPutPerSecond;
                //// summary.ValueMedian = CalcMedian(values);
                return summary;
            }
        } 
    }
}
