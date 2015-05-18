
    1.Overview

        Allows performance/load testing using TestFirst.Net
            
    2. Usage
       
        Example:


            [TestFixture]
            public class MyPerfTest:AbstractNUnitScenarioTest
            {
                [Test]        
                public void WhateverTest()
                {
                    PerformanceMetricsWriter metricsWriter;
                    PerformanceReport report;

                    Scenario()
                        .Given(metricsWriter = PerformanceMetricsWriter.With().TestName("WhateverTest"))
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

            ...
            }   
    
    Any part of the above can be replaced with your own implementation if you don't like what the defaults provide.

