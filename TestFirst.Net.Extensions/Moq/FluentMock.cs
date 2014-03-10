using System;
using System.Linq.Expressions;
using Moq;
using System.Collections.Generic;

namespace TestFirst.Net.Extensions.Moq
{
    /// <summary>
    /// Wraps a <see cref="Mock{T}"/> and provides a fluent interface over it. Recommend to use 
    /// the <see cref="AbstractNUnitMoqScenarioTest.AMock{T}"/> to create an instance of this. Alternatively
    /// create your own but be sure to register this mock with the <see cref="Scenario"/> via injection to ensure
    /// the wrapped mocks <see cref="m_mock.VerifyAll"/> is called at scenario completion. Feel free to implement your
    /// own mechanism but then you maintain it
    /// </summary>
    /// <typeparam name="T">the type of the object we are mocking</typeparam>
    public class FluentMock<T> : IRunOnScenarioEnd, IRunOnMockVerify where T : class 
    {
        private readonly Mock<T> m_mock;
        private readonly List<IRunOnMockVerify> m_runOnVerify = new List<IRunOnMockVerify>(5);

        /// <summary>
        /// Return the instance being mocked. Make all your invoke calls on this
        /// </summary>
        public T Instance
        {
            get { return m_mock.Object; }
        }

        public static FluentMock<T> With()
        {
            return new FluentMock<T>();
        }

        public FluentMock()
        {
            this.m_mock = new Mock<T>(MockBehavior.Strict);
        }

        private FluentMock(Mock<T> mock)
        {
            this.m_mock = mock;
        }

        //for internal use only to allow chaining
        internal FluentMock(FluentMock<T>fluentMock)
        {
            this.m_mock = fluentMock.m_mock;
        }
        
        /// <summary>
        /// Use to create a mock that implements multiple types.
        /// </summary>
        /// <typeparam name="TAnother">Another type implemented by the mock</typeparam>
        /// <returns></returns>
        public FluentMock<TAnother> AndMocks<TAnother>() where TAnother : class
        {
            var methodMock = new FluentMock<TAnother>(m_mock.As<TAnother>());
            RunOnVerify(methodMock);
            return methodMock;
        }

        /// <summary>
        /// Set an expectation on a returning method call just like <see cref="Mock{T}.Setup"/>. 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns>a chaining mock</returns>
        public FluentMethodReturns<T,TResult> WhereMethod<TResult>(Expression<Func<T,TResult>> expression)
        {
            var setup = m_mock.Setup(expression);
            var methodMock = new FluentMethodReturns<T, TResult>(this, setup, expression);
            RunOnVerify(methodMock);
            return methodMock;
        }

        /// <summary>
        /// Set an expectation on a void method call just like <see cref="Mock{T}.Setup"/>
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public FluentMethodVoid<T> WhereMethod(Expression<Action<T>> expression)
        {
            var setup = m_mock.Setup(expression);
            var methodMock = new FluentMethodVoid<T>(this, setup, expression);
            RunOnVerify(methodMock);
            return methodMock;
        }

        /// <summary>
        /// Set an expectation on a getter just like <see cref="Mock{T}.SetupGet{TProperty}"/>
        /// </summary>
        /// <typeparam name="TProperty">the getter type</typeparam>
        /// <param name="expression"></param>
        /// <returns>a chaining mock</returns>
        public FluentPropertyGet<T,TProperty> WhereGet<TProperty>(Expression<Func<T,TProperty>> expression)
        {
            var setup = m_mock.SetupGet(expression);
            var propertyMock = new FluentPropertyGet<T,TProperty>(this,setup, expression);
            RunOnVerify(propertyMock);
            return propertyMock;
        }

        public FluentMock<T> WhereSet(Action<T> setter)
        {
            m_mock.SetupSet(setter);
            return this;
        }

        internal void RunOnVerify(IRunOnMockVerify verifier)
        {
            m_runOnVerify.Add(verifier);
        }

        public void OnScenarioEnd()
        {
            VerifyMock();
        }

        public void VerifyMock()
        {
            foreach (var verifier in m_runOnVerify)
            {
                verifier.VerifyMock();
            }
            m_mock.VerifyAll();
        }        
    }
}