using System;

namespace TestFirst.Net.Util
{
    public class TimeSpanBuilder<TParent>
    {
        private readonly double m_val;

        private readonly Func<TimeSpan, TParent> m_builder;

        public TimeSpanBuilder(double val, Func<TimeSpan, TParent> builder)
        {
            m_val = val;
            m_builder = builder;
        }

        public TParent MilliSeconds()
        {
            return Build(TimeSpan.FromMilliseconds(m_val));
        }
                
        public TParent Seconds()
        {
            return Build(TimeSpan.FromSeconds(m_val));
        }

        public TParent Minutes()
        {
            return Build(TimeSpan.FromMinutes(m_val));
        }

        public TParent Hours()
        {
            return Build(TimeSpan.FromHours(m_val));
        }

        public TParent Days()
        {
            return Build(TimeSpan.FromDays(m_val));
        }

        protected TParent Build(TimeSpan span)
        {
            return m_builder.Invoke(span);
        }
    }
}