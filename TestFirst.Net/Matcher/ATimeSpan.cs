using System;
using TestFirst.Net.Util;

namespace TestFirst.Net.Matcher
{
    public static class ATimeSpan
    {
        public static IMatcher<TimeSpan?> EqualTo(TimeSpan? expect)
        {
            if (expect == null)
            {
                return Null();
            }
            return EqualTo(expect.Value);
        }

        public static FluentMatcherFactory EqualTo(int expect)
        {
            return new SingleArg(expect, EqualTo);
        }

        public static IMatcher<TimeSpan?> EqualTo(TimeSpan expect)
        {
            return Matchers.Function((TimeSpan? actual) => actual == expect, "a TimeSpan == " + expect.ToPrettyString());
        }

        public static FluentMatcherFactory GreaterThan(int expect)
        {
            return new SingleArg(expect, GreaterThan);
        }

        public static IMatcher<TimeSpan?> GreaterThan(TimeSpan expect)
        {
            return Matchers.Function((TimeSpan? actual) => actual.IsValidTimeSpan() && expect.SafeCompareTo(actual) < 0, "a TimeSpan > " + expect.ToPrettyString());
        }

        public static FluentMatcherFactory GreaterOrEqualTo(int expect)
        {
            return new SingleArg(expect, GreaterOrEqualTo);
        }

        public static IMatcher<TimeSpan?> GreaterOrEqualTo(TimeSpan expect)
        {
            return Matchers.Function((TimeSpan? actual) => actual.IsValidTimeSpan() && expect.SafeCompareTo(actual) <= 0, "a TimeSpan >= " + expect.ToPrettyString());
        }

        public static FluentMatcherFactory LessThan(int expect)
        {
            return new SingleArg(expect, LessThan);
        }

        public static IMatcher<TimeSpan?> LessThan(TimeSpan expect)
        {
            return Matchers.Function((TimeSpan? actual) => actual.IsValidTimeSpan() && expect.SafeCompareTo(actual) > 0, "a TimeSpan < " + expect.ToPrettyString());
        }

        public static FluentMatcherFactory LessThanOrEqualTo(int expect)
        {
            return new SingleArg(expect, LessThanOrEqualTo);
        }

        public static IMatcher<TimeSpan?> LessThanOrEqualTo(TimeSpan expect)
        {
            return Matchers.Function((TimeSpan? actual) => actual.IsValidTimeSpan() && expect.SafeCompareTo(actual) >= 0, "a TimeSpan <= " + expect.ToPrettyString());
        }

        public static FluentMatcherFactory Not(int expect)
        {
            return new SingleArg(expect, Not);
        }

        public static IMatcher<TimeSpan?> Not(TimeSpan expect)
        {
            return Matchers.Function((TimeSpan? actual) => actual.IsValidTimeSpan() && actual != expect, "a TimeSpan != " + expect.ToPrettyString());
        }

        public static FluentMatcherFactory Between(int expectFrom, int expectTo)
        {
            return new DualArg(expectFrom, expectTo, Between);
        }

        public static IMatcher<TimeSpan?> Between(TimeSpan expectFrom, TimeSpan expectTo)
        {
            return Matchers.Function((TimeSpan? actual) => actual.IsValidTimeSpan() && expectFrom.SafeCompareTo(actual) < 0 && expectTo.SafeCompareTo(actual) > 0, "a TimeSpan where " + expectFrom.ToPrettyString() + " < value < " + expectTo.ToPrettyString());
        }

        public static FluentMatcherFactory BetweenIncluding(int expectFrom, int expectTo)
        {
            return new DualArg(expectFrom, expectTo, BetweenIncluding);
        }

        public static IMatcher<TimeSpan?> BetweenIncluding(TimeSpan expectFrom, TimeSpan expectTo)
        {
            return Matchers.Function((TimeSpan? actual) => actual.IsValidTimeSpan() && expectFrom.SafeCompareTo(actual) <= 0 && expectTo.SafeCompareTo(actual) >= 0, "a TimeSpan where " + expectFrom.ToPrettyString() + " <= value <= " + expectTo.ToPrettyString());
        }

        public static IMatcher<TimeSpan?> Null()
        {
            return Matchers.Function((TimeSpan? actual) => actual == null, "a null TimeSpan");
        }

        public static IMatcher<TimeSpan?> NotNull()
        {
            return Matchers.Function((TimeSpan? actual) => actual != null, "a non null TimeSpan");
        }
    }

    public abstract class FluentMatcherFactory
    {
        public IMatcher<TimeSpan?> Milliseconds()
        {
            return Matcher(time => TimeSpan.FromMilliseconds(time));
        }

        public IMatcher<TimeSpan?> Seconds()
        {
            return Matcher(time => TimeSpan.FromSeconds(time));
        }

        public IMatcher<TimeSpan?> Minutes()
        {
            return Matcher(time => TimeSpan.FromMinutes(time));
        }

        public IMatcher<TimeSpan?> Hours()
        {
            return Matcher(time => TimeSpan.FromHours(time));
        }

        public IMatcher<TimeSpan?> Days()
        {
            return Matcher(time => TimeSpan.FromDays(time));
        }

        protected abstract IMatcher<TimeSpan?> Matcher(Func<int, TimeSpan> func);
    }

    class SingleArg : FluentMatcherFactory
    {
        private readonly int m_time;

        private readonly Func<TimeSpan, IMatcher<TimeSpan?>> m_matcherCreateFunc;

        internal SingleArg(int time, Func<TimeSpan, IMatcher<TimeSpan?>> matcherCreateFunc)
        {
            m_time = time;
            m_matcherCreateFunc = matcherCreateFunc;
        }

        protected override IMatcher<TimeSpan?> Matcher(Func<int, TimeSpan> func)
        {
            return m_matcherCreateFunc.Invoke(func.Invoke(m_time));
        }
    }

    class DualArg:FluentMatcherFactory
    {
        private readonly int m_time1;
        private readonly int m_time2;

        private readonly Func<TimeSpan,TimeSpan, IMatcher<TimeSpan?>> m_matcherCreateFunc;

        internal DualArg(int time1,int time2, Func<TimeSpan,TimeSpan, IMatcher<TimeSpan?>> matcherCreateFunc)
        {
            m_time1 = time1;
            m_time2 = time2;
            m_matcherCreateFunc = matcherCreateFunc;
        }

        protected override IMatcher<TimeSpan?> Matcher(Func<int,TimeSpan> func)
        {
            return m_matcherCreateFunc.Invoke(func.Invoke(m_time1),func.Invoke(m_time2));
        }
    }

    internal static class TimeSpanExtensions
    {
        internal static bool IsValidTimeSpan(this TimeSpan? val)
        {
            return val!= null;
        }

        internal static int SafeCompareTo(this TimeSpan val, TimeSpan? other)
        {
            if (other == null)
            {
                return -1; //treat null as less
            }
            int compare = val.CompareTo(other.Value);
            return compare;
        }
    }
}
