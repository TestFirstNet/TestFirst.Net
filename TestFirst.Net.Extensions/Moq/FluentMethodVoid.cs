using System;
using System.Linq.Expressions;
using Moq.Language.Flow;
using TestFirst.Net.Util;

namespace TestFirst.Net.Extensions.Moq
{
    public class FluentMethodVoid<T> : FluentMock<T> 
        where T : class
    {
        private readonly ISetup<T> m_setup;
        private readonly Expression<Action<T>> m_expression;
        
        internal FluentMethodVoid(FluentMock<T> mock, ISetup<T> setup, Expression<Action<T>> expression)
                : base(mock)
        {
            m_setup = setup;
            m_expression = expression;
        }

        public FluentMock<T> Throws<TException>()
            where TException : Exception, new() 
        {
            m_setup.Throws<TException>();
            return this;
        }

        public FluentMock<T> Throws(Exception e)
        {
            m_setup.Throws(e);
            return this;
        }

        public FluentMock<T> Throws<TException>(IBuilder<TException> builder) where TException : Exception
        {
            m_setup.Throws(builder.Build());
            return this;
        }

        public TimesBuilder<int, FluentMock<T>> IsCalled(int num)
        {
            return new TimesBuilder<int, FluentMock<T>>(
                num, 
                val => 
                {
                    var counter = new InvocationCounter(val, m_expression);
                    m_setup.Callback(counter.Increment);
                    RunOnVerify(counter);
                    return this; // return the fluent method builder
                });
        }
    }
}