using System;
using Moq.Language.Flow;
using TestFirst.Net.Util;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace TestFirst.Net.Extensions.Moq
{
    /// <typeparam name="T">the type of the class being mocked</typeparam>
    /// <typeparam name="TProperty">the type of the property</typeparam>
    public class FluentPropertyGet<T, TProperty> : IRunOnMockVerify
        where T : class
    {
        private readonly FluentMock<T>  m_mock;
        private readonly ISetupGetter<T, TProperty> m_setup;
        private readonly Expression<Func<T, TProperty>> m_expression;
        private readonly List<IRunOnMockVerify> m_runOnVerify = new List<IRunOnMockVerify>(1);

        internal FluentPropertyGet(FluentMock<T> mock, ISetupGetter<T, TProperty> setup, Expression<Func<T, TProperty>> expression)
        {
            m_mock = mock;
            m_setup = setup;
            m_expression = expression;
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

        public TimesBuilder<int, FluentPropertyGet<T, TProperty>> IsCalled(int num)
        {
            return new TimesBuilder<int, FluentPropertyGet<T, TProperty>>(num, (val) =>
            {
                var counter = new InvocationCounter(val, m_expression);
                m_setup.Callback(counter.Increment);
                RunOnVerify(counter);
                return this;//return the fluent method builder
            });
        }

        internal void RunOnVerify(IRunOnMockVerify verifier)
        {
            m_runOnVerify.Add(verifier);
        }

        public void VerifyMock()
        {
            foreach (var verifier in m_runOnVerify)
            {
                verifier.VerifyMock();
            }
        }
    }
}