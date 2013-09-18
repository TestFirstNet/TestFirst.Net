using System;
using TestFirst.Net.Extensions.Matcher;
using TestFirst.Net.Extensions.NUnit;

namespace TestFirst.Net.Extensions.Moq
{
    /// <summary>
    /// Extend this test case to provide easy creation and registration of Moq mocks.
    /// 
    /// If you use the fluent moqs, and the <see cref="MatcherMoqExtensions"/> then a fullish example would be
    /// 
    /// [TestFixture]
    /// MyTestClass : AbstractNUnitMoqScenarioTest {
    /// 
    ///     [Test]
    ///     public void myTest(){
    ///         
    ///         MyClass foo;
    ///         String response;
    /// 
    ///         scenario() //this will set the scenario name to the name of your test method, in this case 'myTest'
    ///             .Given(foo=AMock&lt;MyClass&gt;() //setup the mock
    ///                     .WhereMethod(f=>f.DoIt(
    ///                         AString.EndingWith("It").Verify(),
    ///                         AnInt.GreaterThan(0).Verify()
    ///                     )
    ///                     .Returns("done!")
    ///                     .WhereMethod(f=>f.DoneIt(
    ///                         AString.EqualTo("done!").Verify()
    ///                     )
    ///                     .Instance
    ///             )
    ///             .When(response=foo.DoIt("WorkIt",2)) //invoke the 1st method
    ///             .Then(ExpectThat(response),Is(AString.EqualTo("done!"))
    ///             .When(foo.DoneIt(response)) //invoke the 2nd method
    ///             .Then(Nothing())//you shoud assert something here
    ///            
    ///          //Moq's Mock.VerifyAll will be automatically called at the end of the scenario
    ///     }
    /// 
    /// }
    /// </summary>
    public abstract class AbstractNUnitMoqScenarioTest : AbstractNUnitScenarioTest
    {
        private bool m_mocksDefined;

        /// <summary>
        /// Create a mock for the given type, ensuring the mock is verified at scenario completion. That is,
        /// Moq's Mock.VerifAll will be called on the underlying mock at the end of the scenario
        /// 
        /// </summary>
        /// <typeparam name="T">the type to create the mock for</typeparam>
        /// <returns>a fluent mock</returns>
        protected FluentMock<T> AMock<T>() where T:class
        {
            AssertInScenario();
            var mock = new FluentMock<T>();
            //ensure mock is verified at senario completion
            InjectDependencies(mock);
            m_mocksDefined = true;
            return mock;
        }

        public override void BeforeTest()
        {
            base.BeforeTest();
            //reset so no unexpected consequences if left in by mistake
            Arg.UseVerifyToConsole(false);
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

        protected T ArgIs<T>(IMatcher<T?> matcher) where T:struct
        {
            return Arg.Is(matcher);
        }

        protected T? ArgIsNotNull<T>(T val) where T:struct
        {
            return Arg.IsNotNull(val);
        }
        /// <summary>
        /// Helper to be used in the last .Then(..) when using mocks and no other assertions have been made. The mocks
        /// themselves should perform the assertions so there is no need to have the scenario perform it. However since
        /// the scenario always requires atleast one Then(..) assertion by design, we need to supply this one.
        /// 
        /// This will fail when no mocks have been defined via the <see cref="AMock{T}"/>. It performs a simple mock count
        /// </summary>
        /// <returns></returns>
        protected Action ExpectNoMocksFailed()
        {
            return () => TestFirstAssert.IsTrue(m_mocksDefined, "No FluentMocks created. Create atleast one via .AMock<T>()");
        }      
    }
}
