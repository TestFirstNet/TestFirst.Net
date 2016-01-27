using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Moq.Language.Flow;
using TestFirst.Net.Util;

namespace TestFirst.Net.Extensions.Moq
{
    /// <summary>Fluent mock builder for describing return value of method</summary>
    /// <typeparam name="T">the type of the class being mocked</typeparam>
    /// <typeparam name="TResult">the return type of the method</typeparam>
    public class FluentMethodReturns<T, TResult> : IRunOnMockVerify
        where T : class
    {
        private readonly FluentMock<T> m_mock;
        private readonly ISetup<T, TResult> m_setup;
        private readonly int m_numArgs;
        private readonly Expression<Func<T, TResult>> m_expression;
        private readonly List<IRunOnMockVerify> m_runOnVerify = new List<IRunOnMockVerify>(1);

        internal FluentMethodReturns(FluentMock<T> mock, ISetup<T, TResult> setup, Expression<Func<T, TResult>> expression)
        {
            var call = expression.Body as MethodCallExpression;
            m_mock = mock;
            m_setup = setup;
            m_numArgs = call == null ? 0 : call.Arguments.Count;
            m_expression = expression;
        }

        public FluentMock<T> ReturnsNull()
        {
            m_setup.Returns(null);
            return m_mock;
        }
        
        public FluentMock<T> ReturnsInSequence(params TResult[] values)
        {
            var returner = new InSequenceReturner<TResult>(values);
            switch (m_numArgs)
            {
                case 0: 
                    m_setup.Returns(returner.NextValue);
                    break;
                case 1: 
                    m_setup.Returns(new Func<object, TResult>(arg0 => returner.NextValue()));
                    break;
                case 2:
                    m_setup.Returns(new Func<object, object, TResult>((arg0, arg1) => returner.NextValue()));
                    break;
                case 3:
                    m_setup.Returns(new Func<object, object, object, TResult>((arg0, arg1, arg2) => returner.NextValue()));
                    break;
                case 4:
                    m_setup.Returns(new Func<object, object, object, object, TResult>((arg0, arg1, arg2, arg3) => returner.NextValue()));
                    break;
                case 5:
                    m_setup.Returns(new Func<object, object, object, object, object, TResult>((arg0, arg1, arg2, arg3, arg4) => returner.NextValue()));
                    break;
                case 6:
                    m_setup.Returns(new Func<object, object, object, object, object, object, TResult>((arg0, arg1, arg2, arg3, arg4, arg5) => returner.NextValue()));
                    break;
                default:
                    throw new TestFirstException("This is crazy, why do you have methods with more than 6 args? Currently only support up to 6. Time to refactor!");
            }

            return m_mock;
        }
        
        public FluentMock<T> Returns(TResult result)
        {
            m_setup.Returns(result);
            return m_mock;
        }

        public FluentMock<T> ReturnsSelf()
        {            
            if (typeof(TResult).IsAssignableFrom(typeof(T)))
            {
                // use lazy creation to allow Moq to setup the instance first until we need it
                m_setup.Returns(() => 
                {
                    object self = m_mock.Instance;
                    return (TResult)self;
                });
            }
            else
            {
                throw new TestFirstException(string.Format("Type {0} is not castable to method return type {1}", typeof(T).FullName, typeof(TResult).FullName));
            }
            return m_mock;
        }

        public FluentMock<T> Returns<TMethodArg>(Func<TMethodArg, TResult> valueFunction)
        {
            m_setup.Returns(valueFunction);
            return m_mock;
        }

        public FluentMock<T> Returns<TMethodArg0, TMethodArg1>(Func<TMethodArg0, TMethodArg1, TResult> valueFunction)
        {
            m_setup.Returns(valueFunction);
            return m_mock;
        }

        public FluentMock<T> Returns<TMethodArg0, TMethodArg1, TMethodArg2>(Func<TMethodArg0, TMethodArg1, TMethodArg2, TResult> valueFunction)
        {
            m_setup.Returns(valueFunction);
            return m_mock;
        }

        public FluentMock<T> Returns<TMethodArg0, TMethodArg1, TMethodArg2, TMethodArg3>(Func<TMethodArg0, TMethodArg1, TMethodArg2, TMethodArg3, TResult> valueFunction)
        {
            m_setup.Returns(valueFunction);
            return m_mock;
        }

        public FluentMock<T> Returns(IBuilder<TResult> builder)
        {
            m_setup.Returns(builder.Build());
            return m_mock;
        }

        public FluentMock<T> Throws<TException>()
            where TException : Exception, new() 
        {
            m_setup.Throws<TException>();
            return m_mock;
        }

        public FluentMock<T> Throws(Exception e)
        {
            m_setup.Throws(e);
            return m_mock;
        }

        public FluentMock<T> Throws<TException>(IBuilder<TException> builder) 
            where TException : Exception
        {
            m_setup.Throws(builder.Build());
            return m_mock;
        }

        public FluentMethodReturns<T, TResult> Executes(Action action)
        {
            m_setup.Callback(action);
            return this;
        }

        public TimesBuilder<int, FluentMethodReturns<T, TResult>> IsCalled(int num)
        {
            return new TimesBuilder<int, FluentMethodReturns<T, TResult>>(
                num, 
                val =>
                    {
                        var counter = new InvocationCounter(val, m_expression);
                        m_setup.Callback(counter.Increment);
                        RunOnVerify(counter);
                        return this; // return the fluent method builder
                    });
        }

        public void VerifyMock()
        {
            foreach (var verifier in m_runOnVerify)
            {
                verifier.VerifyMock();
            }
        }

        internal void RunOnVerify(IRunOnMockVerify verifier)
        {
            m_runOnVerify.Add(verifier);
        }
    }
}