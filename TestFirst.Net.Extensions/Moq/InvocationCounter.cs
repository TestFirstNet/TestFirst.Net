using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;

namespace TestFirst.Net.Extensions.Moq
{
    internal class InvocationCounter : IRunOnMockVerify
    {
        private readonly LambdaExpression m_methodExpression;
        private readonly int m_expectCount;
        private int m_invokeCount;

        internal InvocationCounter(int expect, LambdaExpression expression)
        {
            m_expectCount = expect;
            m_methodExpression = expression;
        }

        internal void Increment()
        {
            var count = Interlocked.Increment(ref m_invokeCount);
            if (count > m_expectCount)//lets bail as soon as possible
            {
                throw new AssertionFailedException("Invocation " + m_methodExpression + " was unexpectedly performed " + Times(count) + ", but expected " + Times(m_expectCount));
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
                throw new AssertionFailedException("Invocation " + m_methodExpression + " was unexpectedly performed " + Times(m_invokeCount) + ", but expected " + Times(m_expectCount));
            }
        }
    }

}
