using System;
using System.Text;
using TestFirst.Net.Matcher.Internal;
using TestFirst.Net.Util;

namespace TestFirst.Net.Matcher
{
    public static class ADateTimeOffset
    {
        public static IMatcher<DateTimeOffset?> Any()
        {
            return AnInstance.Any<DateTimeOffset?>();
        }

        public static DateTimeOffsetIntervalMatcher Before(DateTimeOffset? expect)
        {
            return new DateTimeOffsetIntervalMatcher().Before(expect);
        }

        public static DateTimeOffsetIntervalMatcher Before(DateTimeOffset? expect, int millisecondsBuffer)
        {
            if (millisecondsBuffer > 0)
                millisecondsBuffer = -1 * millisecondsBuffer;

            if (expect != null)
                expect = expect.Value.AddMilliseconds(millisecondsBuffer);

            return new DateTimeOffsetIntervalMatcher().Before(expect);
        }

        public static DateTimeOffsetIntervalMatcher After(DateTimeOffset? expect)
        {
            return new DateTimeOffsetIntervalMatcher().After(expect);
        }

        public static DateTimeOffsetIntervalMatcher After(DateTimeOffset? expect, int millisecondsBuffer)
        {
            if (millisecondsBuffer > 0)
                millisecondsBuffer = -1 * millisecondsBuffer;

            if (expect != null)
                expect = expect.Value.AddMilliseconds(millisecondsBuffer);

            return new DateTimeOffsetIntervalMatcher().After(expect);
        }
        public static DateTimeOffsetEqualsMatcher Now()
        {
            return new DateTimeOffsetEqualsMatcher().EqualTo(DateTimeOffset.Now);
        }

        public static DateTimeOffsetEqualsMatcher EqualTo(DateTimeOffset? expect)
        {
            return new DateTimeOffsetEqualsMatcher().EqualTo(expect);
        }

        public static DateTimeOffsetEqualsMatcher FromNow(TimeSpan span)
        {
            return new DateTimeOffsetEqualsMatcher().EqualTo(DateTimeOffset.Now.Add(span));
        }

        public static IMatcher<DateTimeOffset?> Null()
        {
            return Matchers.Function((DateTimeOffset? actual) => actual == null, "a null DateTimeOffset");
        }

        public static IMatcher<DateTimeOffset?> NotNull()
        {
            return Matchers.Function((DateTimeOffset? actual) => actual != null, "a non null DateTimeOffset");
        }

        public class DateTimeOffsetEqualsMatcher : AbstractMatcher<DateTimeOffset?>, IProvidePrettyTypeName
        {
            private DateTimeOffset? m_expect;
            private TimeSpan? m_maxMinus;
            private TimeSpan? m_maxPlus;
            private bool m_inclusive = true;

            private IMatcher<DateTimeOffset> m_cachedMatcher;

            internal DateTimeOffsetEqualsMatcher()
            {
            }

            public TimeSpanBuilder<DateTimeOffsetEqualsMatcher> Within(double val)
            {
                return new TimeSpanBuilder<DateTimeOffsetEqualsMatcher>(val, span => Within(span));
            }

            /// <summary>
            /// The error margin on the match. Same as calling PlusMax and MinusMax together with the same value
            /// </summary>
            /// <param name="span">The error margin on the match</param>
            /// <returns>A DateTimeOffset matcher with a margin of error</returns>
            public DateTimeOffsetEqualsMatcher Within(TimeSpan span)
            {
                MinusMax(span);
                PlusMax(span);
                return this;
            }

            /// <summary>
            /// Add an upper bound to the error margin. If the datetime is more than this from the required equals, then fail. Depends on the 
            /// Inclusive/Exclusive flag whether the actual bound is included
            /// </summary>
            /// <param name="val">The upper bound on the error margin</param>
            /// <returns>A timespan builder for creating a DateTimeOffset matcher</returns>
            public TimeSpanBuilder<DateTimeOffsetEqualsMatcher> PlusMax(double val)
            {
                return new TimeSpanBuilder<DateTimeOffsetEqualsMatcher>(val, span => PlusMax(span));
            }

            public DateTimeOffsetEqualsMatcher PlusMax(TimeSpan span)
            {
                m_maxPlus = span;
                return this;
            }

            /// <summary>
            /// Add a lower bound to the error margin. If the datetime is more than this from the required equals, then fail. Depends on the 
            /// Inclusive/Exclusive flag whether the actual bound is included
            /// </summary>
            /// <param name="val">The lower bound on the error margin</param>
            /// <returns>A timespan builder for creating a DateTimeOffset matcher</returns>
            public TimeSpanBuilder<DateTimeOffsetEqualsMatcher> MinusMax(double val)
            {
                return new TimeSpanBuilder<DateTimeOffsetEqualsMatcher>(val, span => MinusMax(span));
            }

            /// <summary>
            /// Add a lower bound to the error margin
            /// </summary>
            /// <param name="span">The lower bound on the error margin</param>
            /// <returns>A timespan builder for creating a DateTimeOffset matcher</returns>
            public DateTimeOffsetEqualsMatcher MinusMax(TimeSpan span)
            {
                m_maxMinus = span;
                return this;
            }

            /// <summary>
            /// Include the bounds
            /// </summary>
            /// <returns>A DateTimeOffset matcher which includes the bounds</returns>
            public DateTimeOffsetEqualsMatcher Inclusive()
            {
                m_inclusive = true;
                ResetCached();
                return this;
            }

            /// <summary>
            /// Exclude the bounds
            /// </summary>
            /// <returns>A DateTimeOffset matcher which excludes the bounds</returns>
            public DateTimeOffsetEqualsMatcher Exclusive()
            {
                m_inclusive = false;
                ResetCached();
                return this;
            }

            public override bool Matches(DateTimeOffset? actual, IMatchDiagnostics diagnostics)
            {
                if (actual == null && m_expect == null)
                {
                    diagnostics.Matched("Null");
                    return true;
                }
                if (actual == null)
                {
                    diagnostics.MisMatched("Was null");
                    return false;
                }
                if (m_expect == null)
                {
                    diagnostics.MisMatched("Expected null");
                    return false;
                }
                return diagnostics.TryMatch(actual, GetOrBuild());
            }

            public override void DescribeTo(IDescription desc)
            {
                var sb = new StringBuilder("A DateTimeOffset equal to ");
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
                return "ADateTimeOffset";
            }

            internal DateTimeOffsetEqualsMatcher EqualTo(DateTimeOffset? expect)
            {
                m_expect = expect;
                return this;
            }

            private void ResetCached()
            {
                m_cachedMatcher = null;
            }

            private IMatcher<DateTimeOffset> GetOrBuild()
            {
                var cached = m_cachedMatcher;
                if (cached == null)
                {
                    cached = Matchers.Function((DateTimeOffset actual, IMatchDiagnostics diagnostics) => MatchFunction(actual, diagnostics), string.Empty);
                    m_cachedMatcher = cached;
                }
                return cached;
            }

            private bool MatchFunction(DateTimeOffset actual, IMatchDiagnostics diagnostics)
            {
                var expectVal = m_expect.Value;
                var plus = m_inclusive ? 1 : 0;
                var maxMinus = Math.Truncate((m_maxMinus ?? TimeSpan.FromMilliseconds(0)).TotalMilliseconds + plus);
                var maxPlus = Math.Truncate((m_maxPlus ?? TimeSpan.FromMilliseconds(0)).TotalMilliseconds + plus);

                IMatcher<double?> diffMatcher = ADouble.BetweenIncluding(-maxMinus, maxPlus);
                double diff = Math.Truncate((actual - expectVal).TotalMilliseconds);
                return diffMatcher.Matches(diff, diagnostics);
            }
        }

        public class DateTimeOffsetIntervalMatcher : AbstractMatcher<DateTimeOffset?>, IProvidePrettyTypeName
        {
            private TimeSpan m_expectWithin = TimeSpan.FromMilliseconds(0);
            private DateTimeOffset? m_expectAfter;
            private DateTimeOffset? m_expectBefore;
            private TimeSpan? m_offset;

            private bool m_inclusive;

            internal DateTimeOffsetIntervalMatcher()
            {
            }

            public DateTimeOffsetIntervalMatcher AfterNow()
            {
                After(DateTime.Now);
                return this;
            }

            public DateTimeOffsetIntervalMatcher After(DateTimeOffset? expect)
            {
                m_expectAfter = expect;
                return this;
            }
            
            public DateTimeOffsetIntervalMatcher BeforeNow()
            {
                Before(DateTime.Now);
                return this;
            }

            public DateTimeOffsetIntervalMatcher Before(DateTimeOffset? expect)
            {
                m_expectBefore = expect;
                return this;
            }

            public TimeSpanBuilder<DateTimeOffsetIntervalMatcher> Within(double val)
            {
                return new TimeSpanBuilder<DateTimeOffsetIntervalMatcher>(val, span => Within(span));
            }

            public DateTimeOffsetIntervalMatcher Within(TimeSpan span)
            {
                m_expectWithin = span;
                return this;
            }

            public DateTimeOffsetIntervalMatcher Inclusive()
            {
                m_inclusive = true;
                return this;
            }

            public DateTimeOffsetIntervalMatcher Exclusive()
            {
                m_inclusive = false;
                return this;
            }

            public DateTimeOffsetIntervalMatcher Offset(TimeSpan offset)
            {
                m_offset = offset;
                return this;
            }

            public override bool Matches(DateTimeOffset? actual, IMatchDiagnostics diagnostics)
            {
                if (actual == null)
                {
                    diagnostics.MisMatched("Null");
                    return false;
                }
                var within = m_expectWithin.TotalMilliseconds + (m_inclusive ? 1 : 0); // if not inclusive let's be just a little more

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
                var sb = new StringBuilder("A DateTimeOffset where ");
                var sign = m_inclusive ? "= " : " ";

                if (m_expectAfter.HasValue)
                {
                    sb.Append(ApplyOffset(m_expectAfter).ToPrettyString()).Append(" <").Append(sign);
                }
                sb.Append("value");
                if (m_expectBefore.HasValue)
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

            public string GetPrettyTypeName()
            {
                return "ADateTimeOffset";
            }

            private DateTimeOffset ApplyOffset(DateTimeOffset? dt)
            {
                if (m_offset == null)
                {
                    return dt.Value;
                }
                return dt.Value.Add(m_offset.Value);
            }
        }
    }
}
