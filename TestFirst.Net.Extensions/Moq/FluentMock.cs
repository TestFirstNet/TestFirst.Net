using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Castle.DynamicProxy;
using Moq;

namespace TestFirst.Net.Extensions.Moq
{
    /// <summary>
    /// Wraps a <see cref="Mock{T}"/> and provides a fluent interface over it. Recommend to use 
    /// the <see cref="AbstractNUnitMoqScenarioTest.AMock{T}"/> to create an instance of this. Alternatively
    /// create your own but be sure to register this mock with the <see cref="Scenario"/> via injection to ensure
    /// the wrapped mocks <see cref="Mock{T}.VerifyAll"/> is called at scenario completion. Feel free to implement your
    /// own mechanism but then you maintain it
    /// </summary>
    /// <typeparam name="T">the type of the object we are mocking</typeparam>
    public class FluentMock<T> : IRunOnScenarioEnd, IRunOnMockVerify where T : class 
    {
        private static readonly ProxyGenerator Generator = new ProxyGenerator();
        private readonly Mock<T> m_mock;
        private readonly List<IRunOnMockVerify> m_runOnVerify = new List<IRunOnMockVerify>(5);
        private readonly T m_wrapper;

        public FluentMock()
        {
            m_mock = new Mock<T>(MockBehavior.Strict);
            m_wrapper = EventProxyWrapper(m_mock.Object);
        }

        // for internal use only to allow chaining
        internal FluentMock(FluentMock<T>fluentMock)
        {
            m_mock = fluentMock.m_mock;
            m_wrapper = EventProxyWrapper(m_mock.Object);
        }

        private FluentMock(Mock<T> mock)
        {
            m_mock = mock;
            m_wrapper = EventProxyWrapper(m_mock.Object);
        }

        /// <summary>
        /// Gets the instance being mocked. Make all your invoke calls on this
        /// </summary>
        public T Instance
        {
            get { return m_wrapper; }
        }

        public static FluentMock<T> With()
        {
            return new FluentMock<T>();
        }

        /// <summary>
        /// Use to create a mock that implements multiple types.
        /// </summary>
        /// <typeparam name="TAnother">Another type implemented by the mock</typeparam>
        /// <returns>The fluent mock builder</returns>
        public FluentMock<TAnother> AndMocks<TAnother>() where TAnother : class
        {
            var methodMock = new FluentMock<TAnother>(m_mock.As<TAnother>());
            RunOnVerify(methodMock);
            return methodMock;
        }

        /// <summary>
        /// Set an expectation on a returning method call just like <see cref="Mock{T}.Setup"/>. 
        /// </summary>
        /// <param name="expression">The expression identifying the method to mock</param>
        /// <returns>a chaining mock</returns>
        /// <typeparam name="TResult">The return type of the method</typeparam>
        public FluentMethodReturns<T, TResult> WhereMethod<TResult>(Expression<Func<T, TResult>> expression)
        {
            var setup = m_mock.Setup(expression);
            var methodMock = new FluentMethodReturns<T, TResult>(this, setup, expression);
            RunOnVerify(methodMock);
            return methodMock;
        }

        /// <summary>
        /// Set an expectation on a void method call just like <see cref="Mock{T}.Setup"/>
        /// </summary>
        /// <param name="expression">The expression to mock</param>
        /// <returns>The fluent mock builder</returns>
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
        /// <param name="expression">The property expression to mock</param>
        /// <returns>a chaining mock</returns>
        public FluentPropertyGet<T, TProperty> WhereGet<TProperty>(Expression<Func<T, TProperty>> expression)
        {
            var setup = m_mock.SetupGet(expression);
            var propertyMock = new FluentPropertyGet<T, TProperty>(this, setup, expression);
            RunOnVerify(propertyMock);
            return propertyMock;
        }

        public FluentMock<T> WhereSet(Action<T> setter)
        {
            m_mock.SetupSet(setter);
            return this;
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

        internal void RunOnVerify(IRunOnMockVerify verifier)
        {
            m_runOnVerify.Add(verifier);
        }

        private static T EventProxyWrapper(T mockedObject)
        {
            return Generator.CreateInterfaceProxyWithTargetInterface(mockedObject, new EventInterceptor());
        }
    }
}