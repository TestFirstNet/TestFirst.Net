using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestFirst.Net.Lang;

namespace TestFirst.Net.Performance
{
    public class PerformanceReport
    {
        private readonly IDictionary<string,MetricSummary> m_metricSummariesByLabel = new Dictionary<string, MetricSummary>();
        
        public String Title { get; set; }
        public DateTime GeneratedAt { get; set; }
        public String MetricsFilePath { get; set; }

        public DateTime? MetricsStartedAt { get; set; }
        public DateTime? MetricsEndedAt { get; set; }

        public PerformanceReport()
        {
            GeneratedAt = DateTime.Now;
        }

        internal void AddSummaries(IEnumerable<MetricSummary> summaries)
        {
            foreach (var summary in summaries)
            {
                AddSummary(summary);
            }
        }
           
        internal void AddSummary(MetricSummary summary)
        {
            m_metricSummariesByLabel[summary.MetricName] = summary;
        }

        public MetricSummary GetMetricSummaryNamed(string metricName)
        {
            PreConditions.AssertTrue(m_metricSummariesByLabel.ContainsKey(metricName), "metric with name" + metricName);
            return m_metricSummariesByLabel[metricName];
        }

        public void PrintToConsole()
        {
            Console.WriteLine(ToString());
        }

        public bool HasErrors()
        {
            return m_metricSummariesByLabel.Values.Any(summary => summary.ErrorCount > 0);
        }

        public override String ToString()
        {
            var sb = new StringBuilder();
            sb.Append("*** Performance Report : ").Append(Title).Append(" ***");
            sb.AppendLine().Append("Generated At: ").Append(GeneratedAt.ToString("yyyy-MM-dd HHmm:ss"));
            sb.AppendLine().Append("     Metrics: \"").Append(MetricsFilePath).Append("\"");

            sb.AppendLine().Append("     Summary:");
            foreach (var summary in m_metricSummariesByLabel.Values)
            {
                sb.AppendLine().Append("For metric '").Append(summary.MetricName).Append("'");
                sb.AppendLine().Append("        Total Metric Count: ").Append(summary.TotalCount);
                sb.AppendLine().Append("                  Ok Count: ").Append(summary.TotalCount - summary.ErrorCount);
                sb.AppendLine().Append("               Error Count: ").Append(summary.ErrorCount).Append(" (ignored in calculations)" );
                sb.AppendLine().Append("         Throughput/Second: ").Append(NoneIfNotSet(summary.ThroughPutPerSecond));
                sb.AppendLine().Append("              Metric Units: ").Append("ms");
                sb.AppendLine().Append("                   Min Val: ").Append(NoneIfNotSet(summary.ValueMin));
                sb.AppendLine().Append("                  Mean Val: ").Append(NoneIfNotSet(summary.ValueMean));
                sb.AppendLine().Append("                Median Val: ").Append(NoneIfNotSet(summary.ValueMedian));                
                sb.AppendLine().Append("                   Max Val: ").Append(NoneIfNotSet(summary.ValueMax));
                sb.AppendLine().Append("              StdDeviation: ").Append(summary.ValueStdDeviation);
                //give an indication of the scale of the variance
                if (summary.ValueMin >= 0 && summary.ValueMean > 0)
                {
                    sb.AppendLine().Append("    StdDeviation % of Mean: ").Append(summary.ValueStdDeviation/summary.ValueMean * 100).Append("%");
                }
                sb.AppendLine().Append("   68% confidence interval (+/-1 std deviation)");
                sb.AppendLine().Append("                          : ");
                sb.Append(summary.ValueMean - summary.ValueStdDeviation);   
                sb.Append(" to ").Append(summary.ValueMean + summary.ValueStdDeviation);
                sb.AppendLine().Append("   95% confidence interval (+/-2 std deviations)");
                sb.AppendLine().Append("                          : ");
                sb.Append(summary.ValueMean - summary.ValueStdDeviation*2);
                sb.Append(" to ").Append(summary.ValueMean + summary.ValueStdDeviation*2);

                //sb.AppendLine().Append("            Upper outliers: ");
                //var maxAllowed = summary.ValueMean + summary.ValueStdDeviation*2;
                //var outliers =summary.RawMetrics
                //    .Select((metric) => metric.Value)
                //    .Where((value) => value > maxAllowed)
                //    .ToList();
                //if (outliers.Count == 0)
                //{
                //    sb.Append("none");
                //}
                //foreach (var outlier in outliers)
                //{
                //    sb.AppendLine().Append("                           ").Append(outlier);
                //}                
            }

            return sb.ToString();
        }

        private static String NoneIfNotSet(double d)
        {
            if (d == Double.MinValue || d == Double.MaxValue)
            {
                return "-";
            }
            return d.ToString();
        }

        public class MetricSummary
        {           
            public string MetricName { get; internal set; }

            public int TotalCount { get; internal set; }
            public int ErrorCount { get; internal set; }
            public double ValueMean { get; internal set; }
            public double ValueStdDeviation { get; internal set; }
            public double ValueMin { get; internal set; }
            public double ValueMax { get; internal set; }        
            public double ValueMedian { get; internal set; }   

            public double ThroughPutPerSecond { get; internal set; }
        }
    }
}