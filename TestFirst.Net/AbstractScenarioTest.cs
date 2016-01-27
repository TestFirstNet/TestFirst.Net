using System;

namespace TestFirst.Net
{
    /// <summary>
    /// Convenience class for creating unit tests using your test framework of choice. Be sure to invoke 
    /// BeforeTest and AfterTest from your tests. It is recommended to create your own base test class and
    /// ensure this always calls these two methods, then have your tests subclass that. 
    /// <para>
    /// See ScenarioHelper for usage and examples
    /// </para>
    /// </summary>
    public abstract class AbstractScenarioTest : ScenarioFluency
    {
        private static readonly IStepArgDependencyInjector DefaultInjector = new NullStepArgDependencyInjector();
        private readonly Rand.Random m_random = new Rand.Random();

        protected AbstractScenarioTest()
        {
            Injector = DefaultInjector;
        }

        /// <summary>
        /// Call me before running your test method
        /// </summary>
        public virtual void BeforeTest()
        {
            CurrentScenario = null;
        }

        /// <summary>
        /// Call me after your test method. This will ensure the scenario's in your test method have all been completed and
        /// passed
        /// </summary>
        public virtual void AfterTest()
        {
            var scenario = CurrentScenario;
            if (scenario != null)
            {
                // don't cause the next scenario to fail because of the previous one
                CurrentScenario = null;
                try
                {
                    scenario.AssertHasRunAndPassed();
                }
                finally
                {
                    scenario.Dispose();
                }
            }
        }

        protected Rand.Random ARandom()
        {
            return m_random;
        }

        protected void UseSimpleStepArgInjector()
        {
            UseScenarioInjector(new SimpleStepArgInjector());
        }

        protected void UseDisposingScenarioInjector()
        {
            UseScenarioInjector(new DisposingStepArgInjector());
        }

        protected void UseScenarioInjector(IStepArgDependencyInjector dependencyInjector)
        {
            Injector = dependencyInjector ?? DefaultInjector;
        }

        /// <summary>
        /// Create a new scenario with the given title, ensuring the previous scenario was properly
        /// cleaned up. Suggested if you use MBUnit or NUnit to
        /// create a base class with a method 'Scenario()' which uses the current test's name. E.g.
        /// <para>
        /// protected Scenario()
        /// {
        ///    return Scenario(TestContext.CurrentContext.Test.Name);
        /// }
        /// </para>
        /// </summary>
        /// <param name="title">the scenario title. Can not be empty or null</param>
        /// <returns>a newly created scenario</returns>
        protected Scenario Scenario(string title)
        {
            // ensure previous scenario has completed to prevent dangling scenarios
            // i.e. those which look like they have passed but haven't actually run
            if (CurrentScenario != null) 
            { 
                try
                {
                    AfterTest();
                }
                catch (AssertionFailedException e)
                {
                    throw new AssertionFailedException("Previous Scenario failed", e);
                }
                BeforeTest();
            }
            if (string.IsNullOrWhiteSpace(title))
            {
                TestFirstAssert.Fail("Scenario's require a title");
            } 
            OnBeforeNewScenario();
            CurrentScenario = new Scenario(title, Injector);      
            return CurrentScenario;
        }

        /// <summary>
        /// Override to perform any custom operations before a new scenario is created
        /// </summary>
        protected virtual void OnBeforeNewScenario()
        {
            // do your own thing here
        }
    }
}
