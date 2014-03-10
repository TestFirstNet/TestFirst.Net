using System;
using Moq.Language.Flow;
using TestFirst.Net.Util;
using System.Threading;
using System.Linq.Expressions;
using Moq;
using System.Collections.Generic;

namespace TestFirst.Net.Extensions.Moq
{
    public class FluentMethodVoid<T>:FluentMock<T> where T : class
    {
        private readonly ISetup<T> m_setup;
        private readonly Expression<Action<T>> m_expression;
        
        internal FluentMethodVoid(FluentMock<T> mock,ISetup<T> setup, Expression<Action<T>> expression):base(mock)
        {
            m_setup = setup;
            m_expression = expression;
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

        public TimesBuilder<int,FluentMock<T>> IsCalled(int num)
        {
            return new TimesBuilder<int,FluentMock<T>>(num,(val)=>{
                var counter = new MetodInvocationCounter(val, m_expression);
                m_setup.Callback(counter.Increment);
                RunOnVerify(counter);
                return this;//return the fluent method builder
            });
        }

        private class MetodInvocationCounter : IRunOnMockVerify
        {
            private readonly Expression<Action<T>> m_expression;
            private readonly int m_expectCount;
            private int m_invokeCount;

            internal MetodInvocationCounter(int expect, Expression<Action<T>> expression){
                m_expectCount = expect;
                m_expression = expression;
            }

            internal void Increment()
            {
                var count = Interlocked.Increment(ref m_invokeCount);
                if (count > m_expectCount)//lets bail as soon as possible
                {
                    throw new AssertionFailedException("Invocation " + m_expression + " was unexpectedly performed " + Times(count) + ", but expected " + Times(m_expectCount));
                }
            }

            private static string Times(int i)
            {
                return i + " " + (i == 1 ? "time" : "times");
            }

            //ensue we handle case where method is nt invoked often enough
            public void VerifyMock()
            {
                if (m_invokeCount != m_expectCount)
                {
                    throw new AssertionFailedException("Invocation " + m_expression + " was unexpectedly performed " + Times(m_invokeCount) + ", but expected " + Times(m_expectCount));
                }
            }
        }
    }
}