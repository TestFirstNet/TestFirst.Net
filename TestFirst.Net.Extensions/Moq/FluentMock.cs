using System;
using System.Linq.Expressions;
using Moq;

namespace TestFirst.Net.Extensions.Moq
{
    /// <summary>
    /// Wraps a <see cref="Mock{T}"/> and provides a fluent interface over it. Recommend to use 
    /// the <see cref="AbstractNUnitMoqScenarioTest.AMock{T}"/> to create an instance of this. Alternatively
    /// create your own but be sure to register this mock with the <see cref="Scenario"/> via injection to ensure
    /// the wrapped mocks <see cref="Mock.VerifyAll"/> is called at scenario completion. Feel free to implement your
    /// own mechanism but then you maintain it
    /// </summary>
    /// <typeparam name="T">the type of the object we are mocking</typeparam>
    public class FluentMock<T> :IRunOnScenarioEnd where T:class 
    {
        private readonly Mock<T> m_mock;
        
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
            return new FluentMock<TAnother>(m_mock.As<TAnother>());
        }

        /// <summary>
        /// Set an expectation on a returning method call just like <see cref="Mock{T}.Setup"/>. 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns>a chaining mock</returns>
        public FluentMethodReturns<T,TResult> WhereMethod<TResult>(Expression<Func<T,TResult>> expression)
        {
            var setup = m_mock.Setup(expression);
            var call = expression.Body as MethodCallExpression;
            if (call != null)
            {
                return new FluentMethodReturns<T, TResult>(this, setup, call.Arguments.Count);
            }
            return new FluentMethodReturns<T, TResult>(this, setup, 0);
        }

        /// <summary>
        /// Set an expectation on a void method call just like <see cref="Mock{T}.Setup"/>
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public FluentMethodVoid<T> WhereMethod(Expression<Action<T>> expression)
        {
            var setup = m_mock.Setup(expression);
            return new FluentMethodVoid<T>(this, setup);
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
            return new FluentPropertyGet<T,TProperty>(this,setup);
        }

        public FluentMock<T> WhereSet(Action<T> setter)
        {
            m_mock.SetupSet(setter);
            return this;
        }

        public virtual void OnScenarioEnd()
        {
            m_mock.VerifyAll();
        }
    }
}