using System;
using System.Text;
using TestFirst.Net.Matcher.Internal;
using TestFirst.Net.Util;

namespace TestFirst.Net.Matcher
{
    public static class ADateTime
    {
        //private const string DateTimeFormat = "yyyy/MM/dd HH:mm:ss.fff";
        //private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static IMatcher<DateTime?> Any()
        {
            return AnInstance.Any<DateTime?>();
        }

        public static DateTimeIntervalMatcher Before(DateTime? expect)
        {
            return new DateTimeIntervalMatcher().Before(expect);
        }

        public static DateTimeIntervalMatcher Before(DateTime? expect, int millisecondsBuffer)
        {
            if (millisecondsBuffer > 0)
                millisecondsBuffer = -1 * millisecondsBuffer;

            if (expect != null)
                expect = expect.Value.AddMilliseconds(millisecondsBuffer);

            return new DateTimeIntervalMatcher().Before(expect);
        }

        public static DateTimeIntervalMatcher After(DateTime? expect)
        {
            return new DateTimeIntervalMatcher().After(expect);
        }

        public static DateTimeIntervalMatcher After(DateTime? expect, int millisecondsBuffer)
        {
            if (millisecondsBuffer > 0)
                millisecondsBuffer = -1*millisecondsBuffer;

            if (expect != null)
                expect = expect.Value.AddMilliseconds(millisecondsBuffer);

            return new DateTimeIntervalMatcher().After(expect);
        }
        public static DateTimeEqualsMatcher Now()
        {
            return new DateTimeEqualsMatcher().EqualTo(DateTime.Now);
        }

        public static DateTimeEqualsMatcher EqualTo(DateTime? expect)
        {
            return new DateTimeEqualsMatcher().EqualTo(expect);
        }

        public static DateTimeEqualsMatcher FromNow(TimeSpan span)
        {
            return new DateTimeEqualsMatcher().EqualTo(DateTime.Now.Add(span));
        }

        public static IMatcher<DateTime?> Null()
        {
            return Matchers.Function((DateTime? actual) => actual == null, "a null DateTime");
        }

        public static IMatcher<DateTime?> NotNull()
        {
            return Matchers.Function((DateTime? actual) => actual != null, "a non null DateTime");
        }

        public class DateTimeEqualsMatcher : AbstractMatcher<DateTime?>, IProvidePrettyTypeName
        {            
            private DateTime? m_expect;
            private TimeSpan? m_maxMinus;
            private TimeSpan? m_maxPlus;
            private bool m_inclusive = true;

            private IMatcher<DateTime> m_cachedMatcher;
             
            internal DateTimeEqualsMatcher()
            {}

            internal DateTimeEqualsMatcher EqualTo(DateTime? expect)
            {
                m_expect = expect;
                m_inclusive = false;
                return this;
            }

            public TimeSpanBuilder<DateTimeEqualsMatcher> Within(double val)
            {
                return new TimeSpanBuilder<DateTimeEqualsMatcher>(val,span=>Within(span));
            }

            /// <summary>
            /// The error margin on the match. Same as calling PlusMax and MinusMax together with the same value
            /// </summary>
            /// <param name="span"></param>
            /// <returns></returns>
            public DateTimeEqualsMatcher Within(TimeSpan span)
            {
                MinusMax(span);
                PlusMax(span);
                return this;
            }

            /// <summary>
            /// Add an upper bound to the error margin. If the datetime is more than this from the required equals, then fail. Depends on the 
            /// Inclusive/Exclusive flag whether the actual bound is included
            /// </summary>
            /// <param name="val"></param>
            /// <returns></returns>
            public TimeSpanBuilder<DateTimeEqualsMatcher> PlusMax(double val)
            {
                return new TimeSpanBuilder<DateTimeEqualsMatcher>(val,span=>PlusMax(span));
            }

            public DateTimeEqualsMatcher PlusMax(TimeSpan span)
            {
                m_maxPlus = span;
                return this;
            }

            /// <summary>
            /// Add a lower bound to the error margin. If the datetime is more than this from the required equals, then fail. Depends on the 
            /// Inclusive/Exclusive flag whether the actual bound is included
            /// </summary>
            /// <param name="val"></param>
            /// <returns></returns>
            public TimeSpanBuilder<DateTimeEqualsMatcher> MinusMax(double val)
            {
                return new TimeSpanBuilder<DateTimeEqualsMatcher>(val,span=>MinusMax(span));
            }

            /// <summary>
            /// Add a lower bound to the error margin
            /// </summary>
            /// <param name="span"></param>
            /// <returns></returns>
            public DateTimeEqualsMatcher MinusMax(TimeSpan span)
            {
                m_maxMinus = span;
                return this;
            }

            /// <summary>
            /// Include the bounds
            /// </summary>
            /// <returns></returns>
            public DateTimeEqualsMatcher Inclusive()
            {
                m_inclusive = true;
                ResetCached();
                return this;
            }

            /// <summary>
            /// Exclude the bounds
            /// </summary>
            /// <returns></returns>
            public DateTimeEqualsMatcher Exclusive()
            {
                m_inclusive = false;
                ResetCached();
                return this;
            }

            private void ResetCached()
            {
                m_cachedMatcher = null;
            }

            public override bool Matches(DateTime? actual, IMatchDiagnostics diagnostics)
            {
                if(actual == null && m_expect == null)
                {
                    diagnostics.Matched("Null");
                    return true;
                }
                if(actual == null)
                {
                    diagnostics.MisMatched("Was null");
                    return false;
                }
                if(m_expect == null)
                {
                    diagnostics.MisMatched("Expected null");
                    return false;
                }
                return diagnostics.TryMatch(actual, GetOrBuild());
            }

            private IMatcher<DateTime> GetOrBuild()
            {
                var cached = m_cachedMatcher;
                if (cached == null)
                {
                    cached = Matchers.Function((DateTime actual, IMatchDiagnostics diagnostics) =>
                        {
                            var expectVal = m_expect.Value;
                            var plus = m_inclusive ? 1 : 0;
                            var maxMinus = Math.Truncate((m_maxMinus ?? TimeSpan.FromMilliseconds(0)).TotalMilliseconds + plus);
                            var maxPlus = Math.Truncate((m_maxPlus ?? TimeSpan.FromMilliseconds(0)).TotalMilliseconds + plus);

                            IMatcher<double?> diffMatcher = ADouble.BetweenIncluding(-maxMinus,maxPlus);
                            double diff = Math.Truncate((actual - expectVal).TotalMilliseconds);
                            return diffMatcher.Matches(diff, diagnostics);
                        }, "");
                    m_cachedMatcher = cached;
                }
                return cached;

            }

            public override void DescribeTo(IDescription desc)
            {
                var sb = new StringBuilder("A DateTime equal to ");
                var sign = m_inclusive ? "= " : " ";
                if (!m_expect.HasValue)
                {
                    sb.Append("Null");
                }
                else
                {
                    sb.Append(m_expect.ToPrettyString());
                    if (m_maxMinus.HasValue || m_maxPlus.HasValue)
                    {
                        sb.Append(" where ");
                        if (m_maxMinus.HasValue)
                        {
                            sb.Append(m_maxMinus.Value.ToPrettyString()).Append(" <").Append(sign);
                        }
                        sb.Append(" max diff ");
                        if (m_maxPlus.HasValue)
                        {
                            sb.Append(" >").Append(sign).Append(m_maxPlus.Value.ToPrettyString());
                        }
                    }
                }
                desc.Text(sb.ToString());
            }

            public string GetPrettyTypeName()
            {
                return "ADateTime";
            }
        }

        public class DateTimeIntervalMatcher : AbstractMatcher<DateTime?>, IProvidePrettyTypeName
        {
            private TimeSpan m_expectWithin = TimeSpan.FromMilliseconds(0);
            private DateTime? m_expectAfter;
            private DateTime? m_expectBefore;
            private TimeSpan? m_offset;

            private bool m_inclusive;

            internal DateTimeIntervalMatcher()
            {}

            public DateTimeIntervalMatcher AfterNow()
            {
                After(DateTime.Now);
                return this;
            }

            public DateTimeIntervalMatcher After(DateTime? expect)
            {
                m_expectAfter = expect;
                return this;
            }
            
            public DateTimeIntervalMatcher BeforeNow()
            {
                Before(DateTime.Now);
                return this;
            }

            public DateTimeIntervalMatcher Before(DateTime? expect)
            {
                m_expectBefore = expect;
                return this;
            }

            public TimeSpanBuilder<DateTimeIntervalMatcher> Within(double val)
            {
                return new TimeSpanBuilder<DateTimeIntervalMatcher>(val,span=>Within(span));
            }

            public DateTimeIntervalMatcher Within(TimeSpan span)
            {
                m_expectWithin = span;
                return this;
            }

            public DateTimeIntervalMatcher Inclusive()
            {
                m_inclusive = true;
                return this;
            }

            public DateTimeIntervalMatcher Exclusive()
            {
                m_inclusive = false;
                return this;
            }

            public DateTimeIntervalMatcher Offset(TimeSpan offset)
            {
                m_offset = offset;
                return this;
            }

            public override bool Matches(DateTime? actual, IMatchDiagnostics diagnostics)
            {
                if(actual == null)
                {
                    diagnostics.MisMatched("Null");
                    return false;
                }
                var within = m_expectWithin.TotalMilliseconds + (m_inclusive?1:0);//if not inclusive let's be just a little more

                if (m_expectAfter.HasValue)
                {
                    double diffPosIfBefore = (ApplyOffset(m_expectAfter) - actual.Value).TotalMilliseconds;
                    if (diffPosIfBefore >= within)
                    {
                        return false;
                    }
                }
                if (m_expectBefore.HasValue)
                {
                    double diffPosIfAfter = (actual.Value - ApplyOffset(m_expectBefore)).TotalMilliseconds;
                    if (diffPosIfAfter >= within)
                    {
                        return false;
                    }
                }
                return true;
            }

            public override void DescribeTo(IDescription desc)
            {
                var sb = new StringBuilder("A DateTime where ");
                var sign = m_inclusive ? "= " : " ";

                if(m_expectAfter.HasValue)
                {
                    sb.Append(ApplyOffset(m_expectAfter).ToPrettyString()).Append(" <").Append(sign);
                }
                sb.Append("value");
                if( m_expectBefore.HasValue)
                {
                    sb.Append(" <").Append(sign).Append(ApplyOffset(m_expectBefore).ToPrettyString());
                }
                sb.Append(" within ").Append(m_expectWithin.ToPrettyString());
                if (m_offset != null)
                {
                    sb.Append(" using offset ").Append(m_offset.Value.ToPrettyString()).Append(" (already applied to before/after limits) ");
                }
                desc.Text(sb.ToString());
            }

            private DateTime ApplyOffset(DateTime? dt)
            {
                if (m_offset == null)
                {
                    return dt.Value;
                }
                return dt.Value.Add(m_offset.Value);
            }

            public string GetPrettyTypeName()
            {
                return "ADateTime";
            }
        }
    }
}
