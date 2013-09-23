using System;
using Moq.Language.Flow;

namespace TestFirst.Net.Extensions.Moq
{
    /// <typeparam name="T">the type of the class being mocked</typeparam>
    /// <typeparam name="TProperty">the type of the property</typeparam>
    public class FluentPropertyGet<T, TProperty> 
        where T : class
    {
        private readonly FluentMock<T>  m_mock;
        private readonly ISetupGetter<T, TProperty> m_setup;
        
        internal FluentPropertyGet(FluentMock<T> mock,ISetupGetter<T,TProperty> setup)
        {
            m_mock = mock;
            m_setup = setup;
        }

        public FluentMock<T> ReturnsNull()
        {
            m_setup.Returns(null);
            return m_mock;
        } 

        public FluentMock<T> ReturnsInSequence(params TProperty[] values)
        {
            var returner = new InSequenceReturner<TProperty>(values);
            return Returns(returner.NextValue);
        }
        
        public FluentMock<T> Returns(TProperty result)
        {
            m_setup.Returns(result);
            return m_mock;
        }


        public FluentMock<T> Returns(Func<TProperty> valueFunction)
        {
            m_setup.Returns(valueFunction);
            return m_mock;
        }

        public FluentMock<T> Returns(IBuilder<TProperty> builder)
        {
            m_setup.Returns(builder.Build());
            return m_mock;
        }

        public FluentMock<T> Throws<TException>()
            where TException : Exception,new() 
        {
            m_setup.Throws<TException>();
            return m_mock;
        }

        public FluentMock<T> Throws(Exception e)
        {
            m_setup.Throws(e);
            return m_mock;
        }

        public FluentMock<T> Throws<TException>(IBuilder<TException> builder) where TException:Exception
        {
            m_setup.Throws(builder.Build());
            return m_mock;
        }
    }
}