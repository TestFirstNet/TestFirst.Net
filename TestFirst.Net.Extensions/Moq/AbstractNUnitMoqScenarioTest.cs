using System;
using TestFirst.Net.Extensions.Matcher;
using TestFirst.Net.Extensions.NUnit;

namespace TestFirst.Net.Extensions.Moq
{
    /// <summary>
    /// Extend this test case to provide easy creation and registration of Moq mocks.
    /// <para>
    /// If you use the fluent moqs, and the <see cref="MatcherMoqExtensions"/> then a fullish example would be
    /// </para>
    /// <para>
    /// [TestFixture]
    /// MyTestClass : AbstractNUnitMoqScenarioTest {
    /// </para>
    /// <para>
    ///     [Test]
    ///     public void myTest(){
    /// </para>
    /// <para>  
    ///         MyClass foo;
    ///         string response;
    /// </para>
    /// <para>
    ///         scenario() // this will set the scenario name to the name of your test method, in this case 'myTest'
    ///             .Given(foo=AMock&lt;MyClass&gt;() // setup the mock
    ///                     .WhereMethod(f => f.DoIt(
    ///                         AString.EndingWith("It").Verify(),
    ///                         AnInt.GreaterThan(0).Verify()
    ///                     )
    ///                     .Returns("done!")
    ///                     .WhereMethod(f => f.DoneIt(
    ///                         AString.EqualTo("done!").Verify()
    ///                     )
    ///                     .Instance
    ///             )
    ///             .When(response=foo.DoIt("WorkIt", 2)) // invoke the first method
    ///             .Then(ExpectThat(response), Is(AString.EqualTo("done!"))
    ///             .When(foo.DoneIt(response)) // invoke the second method
    ///             .Then(Nothing())//you should assert something here
    /// </para>
    /// <para>           
    ///          // Moq's Mock.VerifyAll will be automatically called at the end of the scenario
    ///     }
    /// </para>
    /// }
    /// </summary>
    public abstract class AbstractNUnitMoqScenarioTest : AbstractNUnitScenarioTest
    {
        private bool m_mocksDefined;

        public override void BeforeTest()
        {
            base.BeforeTest();

            // reset so no unexpected consequences if left in by mistake
            Arg.UseVerifyToConsole(false);
        }

        /// <summary>
        /// Create a mock for the given type, ensuring the mock is verified at scenario completion. That is,
        /// Moq's Mock.VerifyAll will be called on the underlying mock at the end of the scenario
        /// </summary>
        /// <typeparam name="T">the type to create the mock for</typeparam>
        /// <returns>a fluent mock</returns>
        protected FluentMock<T> AMock<T>() where T : class
        {
            AssertInScenario();
            var mock = new FluentMock<T>();

            // ensure mock is verified at senario completion
            InjectDependencies(mock);
            m_mocksDefined = true;
            return mock;
        }

        /// <summary>
        /// When using Mocks, print the matcher diagnostics to console. This is helpful to show what did and didn't match rather
        /// than Moq simply stating no match with little additional info on the matches
        /// </summary>
        protected void UseVerifyToConsole()
        {
            Arg.UseVerifyToConsole(true);
        }

        protected T ArgIsAny<T>()
        {
            return Arg.IsAny<T>();
        }

        protected T ArgIs<T>(IMatcher<T> matcher)
        {
            return Arg.Is(matcher);
        }

        protected T ArgIs<T>(IMatcher<T?> matcher) where T : struct
        {
            return Arg.Is(matcher);
        }

        protected T? ArgIsNotNull<T>(T val) where T : struct
        {
            return Arg.IsNotNull(val);
        }

        /// <summary>
        /// Helper to be used in the last .Then(..) when using mocks and no other assertions have been made. The mocks
        /// themselves should perform the assertions so there is no need to have the scenario perform it. However since
        /// the scenario always requires at least one Then(..) assertion by design, we need to supply this one.
        /// <para>
        /// This will fail when no mocks have been defined via the <see cref="AMock{T}"/>. It performs a simple mock count
        /// </para>
        /// </summary>
        /// <returns>An action which asserts that at least one mock has been defined</returns>
        protected Action ExpectNoMocksFailed()
        {
            return () => TestFirstAssert.IsTrue(m_mocksDefined, "No FluentMocks created. Create at least one via .AMock<T>()");
        }      
    }
}
