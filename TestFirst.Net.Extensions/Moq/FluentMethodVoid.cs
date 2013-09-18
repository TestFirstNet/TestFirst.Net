using System;
using Moq.Language.Flow;

namespace TestFirst.Net.Extensions.Moq
{
    public class FluentMethodVoid<T>:FluentMock<T> where T : class
    {
        private readonly ISetup<T> m_setup;

        internal FluentMethodVoid(FluentMock<T> mock,ISetup<T> setup):base(mock)
        {
            m_setup = setup;
        }

        public FluentMock<T> Throws<TException>()
            where TException : Exception,new() 
        {
            m_setup.Throws<TException>();
            return this;
        }

        public FluentMock<T> Throws(Exception e)
        {
            m_setup.Throws(e);
            return this;
        }

        public FluentMock<T> Throws<TException>(IBuilder<TException> builder) where TException:Exception
        {
            m_setup.Throws(builder.Build());
            return this;
        }
    }
}