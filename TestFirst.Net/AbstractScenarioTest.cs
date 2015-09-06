using System;
using TestFirst.Net.Inject;


namespace TestFirst.Net
{
    /// <summary>
    /// Convenience class for creating unit tests using your test framework of choice. Be sure to invoke 
    /// BeforeTest and AfterTest from your tests. It is recommended to create your own base test class and
    /// ensure this always calls these two methods, then have your tests subclass that. 
    /// 
    /// See ScenarioHelper for usage and examples
    /// </summary>
    public abstract class AbstractScenarioTest : ScenarioFluency
    {
        private readonly Rand.Random m_random = new Rand.Random();
        private static readonly ITestInjector DefaultInjector = new NullStepArgDependencyInjector();

        protected AbstractScenarioTest()
        {
            Injector = DefaultInjector;
        }
        
        protected Rand.Random ARandom()
        {
            return m_random;
        }

        [Obsolete("Use UseDefaultInjector")]
        protected void UseSimpleStepArgInjector()
        {
            UseDefaultInjector();
        }


        protected void UseDefaultInjector()
        {
            UseScenarioInjector(new TestInjector());
        }


        protected void UseDisposingScenarioInjector()
        {
            UseScenarioInjector(new DisposingInjector());
        }

        public void UseScenarioInjector(ITestInjector dependencyInjector)
        {
            Injector = dependencyInjector??DefaultInjector;
        }

        /// <summary>
        /// Create a new scenario with the given title, ensuring the previous scenario was properly
        /// cleaned up. Suggested if you use MBUnit or NUnit to
        /// create a base class with a method 'Scenario()' which uses the current test's naame. E.g.
        /// 
        /// protected Scenario(){
        ///    return Scenario(TestContext.CurrentContext.Test.Name);
        /// }
        /// </summary>
        /// <param name="title">the scenario title. Can not be empty or null</param>
        /// <returns>a newly created scenario</returns>
        public Scenario Scenario(String title)
        {
            //ensure previous scenario has completed to prevent dangling scenarios
            //i.e. those which look like they have passed but haven't actually run
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
            if (String.IsNullOrWhiteSpace(title))
            {
                TestFirstAssert.Fail("Scenario's require a title");
            } 
            //Console.WriteLine("New Scenario:" + title);
            OnBeforeNewScenario();
            CurrentScenario = new Scenario(title, Injector);      
            return CurrentScenario;
        }

        /// <summary>
        /// Override to perform any custom operations before a new scenario is created
        /// </summary>
        protected virtual void OnBeforeNewScenario()
        {
            //do your own thing here
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
                //don't cause the next scenario to fail because of the previous one
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

    }
}
