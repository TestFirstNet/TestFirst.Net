using System.Threading;
using NUnit.Framework;
using TestFirst.Net.Extensions.NUnit;
using TestFirst.Net.Matcher;

namespace TestFirst.Net.Performance
{
    [TestFixture]
    public class PerformanceTestRunnerTest:AbstractNUnitScenarioTest
    {
        [Test]        
        public void ReportIsCorrectlyGeneratedTest()
        {
            PerformanceMetricsWriter metricsWriter;
            PerformanceReport report;

            Scenario()
                .Given(metricsWriter = PerformanceMetricsWriter.With().TestName("ReportIsCorrectlyGeneratedTest"))
                .When(PerformanceSuite.With()
                    .NumRuns(2)
                    .PerRunTimeout(20).Seconds()
                    .LoadRunner(ContentionLoadRunner.With()
                        .Tests(new MyPerfTest())
                        .RunTimeout(15).Seconds()                        )           
                    .Listener(metricsWriter)
                    .Build())
                .When(report = metricsWriter.BuildReport())
                .When(report.PrintToConsole)
                .Then(
                    Expect(report.GetMetricSummaryNamed("metric1").ValueMean),
                    Is(ADouble.EqualTo(4.75)))
                .Then(
                    Expect(report.GetMetricSummaryNamed("metric1").ValueMedian),
                    Is(ADouble.EqualTo(5)))
                 .Then(
                    Expect(report.GetMetricSummaryNamed("metric1").ValueMax),
                    Is(ADouble.EqualTo(10)))
                 .Then(
                    Expect(report.GetMetricSummaryNamed("metric1").ValueMin),
                    Is(ADouble.EqualTo(0)))
                 .Then(
                    Expect(report.GetMetricSummaryNamed("metric1").MetricName),
                    Is(AString.EqualTo("metric1")));
        }

        private class MyPerfTest : BasePerformanceTest
        {
            internal static MyPerfTest With()
            {
                return new MyPerfTest();
            }
        
            public override void InvokeTest(IPerformanceTestListener testListener)
            {
                testListener.OnMetric(PerformanceMetric.NameValue("metric1", 0));   
                Thread.Sleep(1);
                testListener.OnMetric(PerformanceMetric.NameValue("metric1", 5));
                Thread.Sleep(1);
                testListener.OnMetric(PerformanceMetric.NameValue("metric1", 4));
                Thread.Sleep(3);
                testListener.OnMetric(PerformanceMetric.NameValue("metric1", 10));
                testListener.OnMetric(PerformanceMetric.NameValue("metricIgnored", 10));                
                testListener.OnMetric(new PerformanceMetric {Name = "metric1", IsError = true});//should be ignored

                testListener.OnMetric(new PerformanceMetric {Name = "metricIgnored2", IsError = true});//single metric with error,average calcs shouldn't bail 


            }
        }
    }
}
