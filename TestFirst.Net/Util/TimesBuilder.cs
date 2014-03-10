using System;

namespace TestFirst.Net.Util
{

    /// <summary>
    /// Provide fluent support for things that happen a given number of times.
    /// 
    /// E.g.
    /// 
    /// matcher.Invoked(3).Times() -> where 'Invoked' returns this builder, and the 'Times' call returns whatever the 
    /// match method 'Invoked' set to be returned on completion (usually the parent builder)
    /// 
    /// </summary>
    /// <typeparam name="TParent"></typeparam>
    public class TimesBuilder<T,TParent>
    {
        private readonly T m_val;

        private readonly Func<T, TParent> m_builder;

        public TimesBuilder(T val, Func<T, TParent> builder)
        {
            m_val = val;
            m_builder = builder;
        }

        public TParent Times()
        {
            return Build(m_val);
        }
        
        protected TParent Build(T val)
        {
            return m_builder.Invoke(val);
        }
    }
}