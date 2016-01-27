using System;
using System.Threading;
using TestFirst.Net.Matcher;
using TestFirst.Net.Util;

namespace TestFirst.Net
{
    /// <summary>
    /// Convenience class for creating unit tests using your test framework of choice. Be sure to invoke 
    /// BeforeTest and AfterTest from your tests. It is recommended to create your own base test class and
    /// ensure this always calls these two methods, then have your tests subclass that. 
    /// <para>
    /// This fluency class will ensure that the Scenario has all the Given, When, Then calls made and fail if not
    /// </para>
    /// <para>
    /// For Example in MBUnit:
    /// </para>
    /// <para>
    /// MyBaseTest{
    /// </para>
    /// <para>
    /// private ScenarioFluency helper = new ScenarioFluency();
    /// </para>
    /// <para>
    /// protected Scenario(string title){
    ///     return helper.Scenario(title);
    /// }
    /// </para>
    /// <para>
    /// [Setup]
    /// public void setup()
    /// {
    ///   helper.BeforeTest();
    /// }
    /// </para>
    /// <para>
    /// [TearDown]
    /// public void cleanup()
    /// {
    ///   helper.AfterTest();
    /// }
    /// </para>
    /// <para>
    /// }
    /// </para>
    /// <para>
    /// and from your subclass you could have
    /// </para>
    /// <para>
    /// MyTest:MyBaseTest {
    /// </para>
    /// <para>
    /// [Test]
    /// public void testAccountDebited()
    /// {
    ///  Scenario("customer account debited")
    ///     .Given(customer = CustomerInTheDb.With().Name("Fred").Balance(12.Dollars())
    ///     .When(CustomerIsCharged(5.Dollars())
    ///     .Then(Expect(ACustomerAccount.With().Id(customer.Id()).BalanceOf(7.Dollars()))
    /// }
    /// }
    /// </para>
    /// </summary>
    public class ScenarioFluency
    {
        private volatile Scenario m_scenario;

        public IStepArgDependencyInjector Injector { get; set; }

        public Scenario CurrentScenario 
        {
            get { return m_scenario; }
            set { m_scenario = value; }
        }

        /// <summary>
        /// Throw exception if not currently in a scenario
        /// </summary>
        public void AssertInScenario()
        {
            if (CurrentScenario == null)
            {
                TestFirstAssert.Fail("No scenario. You can only invoke this operation within the scope of a currently in progress scenario");
            }
        }

        /// <summary>
        /// An action which does nothing to allow empty 'Given', 'When' when these aren't needed
        /// </summary>
        /// <returns>An action which does nothing</returns>
        protected static Action Nothing()
        {
            return () => { /* Do nothing, used for syntatic sugar in tests*/ };
        }

        /// <summary>
        /// Fluent Api syntactic sugar which simply returns the provided instance
        /// </summary>
        /// <param name="instance">The instance</param>
        /// <returns>The given instance</returns>
        /// <typeparam name="T">The type of the object to return</typeparam>
        protected T Is<T>(T instance)
        {
            return instance;
        }

        protected IMatcher<bool?> IsTrue()
        {
            return ABool.True();
        }

        protected IMatcher<bool?> IsFalse()
        {
            return ABool.False();
        }

        /// <summary>
        /// Returns a Matcher which expects null
        /// </summary>
        /// <typeparam name="T">The type of the object to assert is not null</typeparam>
        /// <returns>A matcher</returns>
        protected IMatcher<T> IsANull<T>() where T : class
        {
            return AnInstance.Null<T>();
        }

        /// <summary>
        /// Fluent Api syntactic sugar which simply returns the provided instance
        /// </summary>
        /// <param name="instance">The exception to throw</param>
        /// <typeparam name="T">The type of the exception</typeparam>
        /// <returns>The exception</returns>
        protected T Throws<T>(T instance)
        {
            return instance;
        }

        /// <summary>
        /// Fluent Api syntactic sugar which simply returns the provided instance
        /// </summary>
        /// <param name="instance">The instance to assert on</param>
        /// <typeparam name="T">The type of the instance</typeparam>
        /// <returns>The instance</returns>
        protected T ExpectThat<T>(T instance)
        {
            return instance;
        }

        /// <summary>
        /// Fluent Api syntactic sugar which simply returns the provided instance. Same as <see cref="ExpectThat{T}"/>
        /// </summary>
        /// <param name="instance">The instance to assert on</param>
        /// <typeparam name="T">The type of the instance</typeparam>
        /// <returns>The instance</returns>
        protected T ExpectThatThe<T>(T instance)
        {
            return instance;
        }

        /// <summary>
        /// Fluent Api syntactic sugar which simply returns the provided instance. Same as <see cref="ExpectThat{T}"/>
        /// </summary>
        /// <param name="instance">The instance to assert on</param>
        /// <typeparam name="T">The type of the instance</typeparam>
        /// <returns>The instance</returns>
        protected T Expect<T>(T instance)
        {
            return instance;
        }

        /// <summary>
        /// Fluent Api syntactic sugar which simply returns the provided instance
        /// </summary>
        /// <param name="instance">The instance to assert on</param>
        /// <typeparam name="T">The type of the instance</typeparam>
        /// <returns>The instance</returns>
        protected T That<T>(T instance)
        {
            return instance;
        }

        /// <summary>
        /// Inject dependencies into the given invoker, invoke it, and return the response
        /// </summary>
        /// <param name="invoker">The invokable to invoke</param>
        /// <typeparam name="T">The type returned by the invoker</typeparam>
        /// <returns>The result of invoking the invokable</returns>
        protected T Get<T>(IInvokable<T> invoker)
        {
            InjectDependencies(invoker);
            return invoker.Invoke();
        }

        /// <summary>
        /// Synonym for <see cref="Get{T}(IBuilder{T})"/>
        /// </summary>
        /// <param name="builder">The builder to build</param>
        /// <typeparam name="T">The type built by the builder</typeparam>
        /// <returns>The built object</returns>
        protected T A<T>(IBuilder<T> builder)
        {
            InjectDependencies(builder);
            return builder.Build();
        }

        /// <summary>
        /// Invoke a function which takes in a scenario and returns something
        /// </summary>
        /// <typeparam name="T">The return type of the function</typeparam>
        /// <param name="func">The function to invoke</param>
        /// <returns>The result of the function</returns>
        protected T A<T>(Func<Scenario, T> func)
        {
            InjectDependencies(func);

            return func.Invoke(m_scenario);
        }

        /// <summary>
        /// Inject dependencies into the given builder, invoke it, and return the built object
        /// </summary>
        /// <param name="builder">The builder</param>
        /// <typeparam name="T">The type built by the builder</typeparam>
        /// <returns>The built object</returns>
        protected T Get<T>(IBuilder<T> builder)
        {
            InjectDependencies(builder);
            return builder.Build();
        }

        /// <summary>
        /// Synonym for <see cref="Get{T}(IFetcher{T})"/>
        /// </summary>
        /// <param name="fetcher">The fetcher</param>
        /// <typeparam name="T">The return type of the fetcher</typeparam>
        /// <returns>The fetched object</returns>
        protected T All<T>(IFetcher<T> fetcher)
        {
            InjectDependencies(fetcher);
            return fetcher.Fetch();
        }

        /// <summary>
        /// Inject dependencies into the given fetcher, invoke it, and return the result
        /// </summary>
        /// <param name="fetcher">The fetcher</param>
        /// <typeparam name="T">The return type of the fetcher</typeparam>
        /// <returns>The fetched object</returns>
        protected T Get<T>(IFetcher<T> fetcher)
        {
            InjectDependencies(fetcher);
            return fetcher.Fetch();
        }

        /// <summary>
        /// Inject dependencies into the given instance. Simply delegates to the scenario to do this. Fails if no 
        /// current scenario
        /// </summary>
        /// <param name="instance">The instance to inject dependencies into</param>
        protected virtual void InjectDependencies(object instance)
        {
            AssertInScenario();
            CurrentScenario.InjectDependencies(instance);
        }

        /// <summary>
        /// Invoke the given action, returning the thrown exception, or null if no exception thrown. This is to
        /// allow testing of thrown exceptions.
        /// <para>
        /// Example usage;
        /// </para>
        /// <para>
        ///    scenario()
        ///         ...
        ///         .When(thrown = Catch(() => myThing.MyMethodWhichThrows()))
        ///         .Then(ExpectThat(thrown), Is(...your matcher here...))
        /// </para>
        /// </summary>
        /// <param name="action">The action to invoke which should throw an exception</param>
        /// <returns>The caught exception, or null</returns>
        protected Exception CaughtException(Action action)
        {
            try
            {
                action.Invoke();
                return null;
            }
            catch (Exception e)
            {
                return e;
            }
        }

        protected TException Caught<TException>(Action action) where TException : Exception
        {
            try
            {
                action.Invoke();
                return null;
            }
            catch (TException e)
            {
                return e;
            }
        }

        /// <summary>
        /// Usage: WaitFor(10).Seconds(). Sleeps the current thread for the given time
        /// </summary>
        /// <param name="time">The time to wait for</param>
        /// <returns>The fluent TimeSpanBuilder</returns>
        protected TimeSpanBuilder<Action> WaitFor(int time)
        {
            return new TimeSpanBuilder<Action>(time, WaitFor);
        }

        /// <summary>
        /// Useful when needing to pause test execution (e.g. when running a service) to possibly manually hit endpoints.
        /// </summary>
        /// <param name="time">the time to wait</param>
        /// <returns>an action which will sleep the current thread for the given amount of time</returns>
        protected Action WaitFor(TimeSpan time)
        {
            return () => Thread.Sleep(time);
        }
    }
}