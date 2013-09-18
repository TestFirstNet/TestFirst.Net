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
    /// 
    /// This fluency class will ensure that the Scenario has all the Given,When, Then calls made and fail if not
    /// 
    /// For Example in MBUnit:
    /// 
    /// MyBaseTest{
    /// 
    /// private ScenarioFluency helper = new ScenarioFluency();
    /// 
    /// protected Scenario(string title){
    ///     return helper.Scenario(title);
    /// }
    /// 
    /// [Setup]
    /// public void setup(){
    ///   helper.BeforeTest();
    /// }
    /// 
    /// [TearDown]
    /// public void cleanup(){
    ///   helper.AfterTest();
    /// }
    /// 
    /// }
    /// 
    /// and from your subclass you could have
    /// 
    /// MyTest:MyBaseTest {
    /// 
    /// [Test]
    /// public void testAccountDebited(){
    ///  Scenario("customer account debited")
    ///     .Given(customer = CustomerInTheDb.With().Name("Fred").Balance(12.Dollars())
    ///     .When(CustomerIsCharged(5.Dollars())
    ///     .Then(Expect(ACustomerAccount.With().Id(customer.Id()).BalanceOf(7.Dollars()))
    /// }
    /// }
    ///     
    /// </summary>
    public class ScenarioFluency
    {
        public IStepArgDependencyInjector Injector { get; set; }

        private volatile Scenario m_scenario;

        public Scenario CurrentScenario {
            get { return m_scenario; }
            set { m_scenario = value; }
        }

        /// <summary>
        /// An action which does nothing to allow empty 'Given', 'When' when these aren't needed
        /// </summary>
        protected static Action Nothing()
        {
            return () => { /* Do nothing, used for syntatic sugar in tests*/ };
        }

        /// <summary>
        /// Fluent Api syntactic sugar which simply returns the provided instance
        /// </summary>
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
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected IMatcher<T> IsANull<T>()  where T : class
        {
            return AnInstance.Null<T>();
        }

        /// <summary>
        /// Fluent Api syntactic sugar which simply returns the provided instance
        /// </summary>
        protected T Throws<T>(T instance)
        {
            return instance;
        }

        /// <summary>
        /// Fluent Api syntactic sugar which simply returns the provided instance
        /// </summary>
        protected T ExpectThat<T>(T instance)
        {
            return instance;
        }

        /// <summary>
        /// Fluent Api syntactic sugar which simply returns the provided instance. Same as <see cref="ExpectThat{T}"/>
        /// </summary>
        protected T ExpectThatThe<T>(T instance)
        {
            return instance;
        }

        /// <summary>
        /// Fluent Api syntactic sugar which simply returns the provided instance. Same as <see cref="ExpectThat{T}"/>
        /// </summary>
        protected T Expect<T>(T instance)
        {
            return instance;
        }

        /// <summary>
        /// Fluent Api syntactic sugar which simply returns the provided instance
        /// </summary>
        protected T That<T>(T instance)
        {
            return instance;
        }

        /// <summary>
        /// Inject dependencies into the given invoker, invoke it, and return the response
        /// </summary>
        protected T Get<T>(IInvokable<T> invoker)
        {
            InjectDependencies(invoker);
            return invoker.Invoke();
        }

        /// <summary>
        /// Synonym for <see cref="Get{T}(IBuilder{T})"/>
        /// </summary>
        protected T A<T>(IBuilder<T> builder)
        {
            InjectDependencies(builder);
            return builder.Build();
        }

        /// <summary>
        /// Invoke a function which takes in a scenario and returns something
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        protected T A<T>(Func<Scenario,T> func)
        {
            InjectDependencies(func);

            return func.Invoke(m_scenario);
        }

        /// <summary>
        /// Inject dependencies into the given builder, invoke it, and return the built object
        /// </summary>
        protected T Get<T>(IBuilder<T> builder)
        {
            InjectDependencies(builder);
            return builder.Build();
        }

        /// <summary>
        /// Synonym for <see cref="Get{T}(IFetcher{T})"/>
        /// </summary>
        protected T All<T>(IFetcher<T> fetcher)
        {
            InjectDependencies(fetcher);
            return fetcher.Fetch();
        }

        /// <summary>
        /// Inject dependencies into the given fetcher, invoke it, and return the result
        /// </summary>
        protected T Get<T>(IFetcher<T> fetcher)
        {
            InjectDependencies(fetcher);
            return fetcher.Fetch();
        }

        /// <summary>
        /// Inject dependencies into the given instance. Simply delegates to the scenario to do this. Fails if no 
        /// current scenario
        /// </summary>
        protected virtual void InjectDependencies(Object instance)
        {
            AssertInScenario();
            CurrentScenario.InjectDependencies(instance);
        }

        /// <summary>
        /// Invoke the given action, returning the thrown exception, or null if no exception thrown. This is to
        /// allow testing of thrown exceptions.
        /// 
        /// Example usage;
        /// 
        ///    scenario()
        ///         ...
        ///         .When(thrown = Catch(()=>myThing.MyMethodWhichThrows()))
        ///         .Then(ExpectThat(thrown), Is(...your matcher here...))
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
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

        protected TException Caught<TException>(Action action) where TException:Exception
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
        /// <param name="time"></param>
        /// <returns></returns>
        protected TimeSpanBuilder<Action> WaitFor(int time)
        {
            return new TimeSpanBuilder<Action>(time, WaitFor);
        }

        /// <summary>
        /// Useful when needing to pause test execution (e.g. when running a service) to possibly manully hit endpoints.
        /// 
        /// </summary>
        /// <param name="time">the time to wait</param>
        /// <returns>an action which will sleep the current thread for the given amount of time</returns>
        protected Action WaitFor(TimeSpan time)
        {
            return () => Thread.Sleep(time);
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
    }
}