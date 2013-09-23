using System;
using NUnit.Framework;

namespace TestFirst.Net.Extensions.NUnit
{
    /// <summary>
    /// Extend this test case to provide NUnit support
    /// </summary>
    [TestFixture]
    public abstract class AbstractNUnitScenarioTest : AbstractScenarioTest
    {
        /// <summary>
        /// Return a scenario with the name of the current test method
        /// </summary>
        /// <returns></returns>
        protected Scenario Scenario()
        {
            try
            {
                var nunitTestName = TestContext.CurrentContext.Test.Name;
                if (nunitTestName == null)
                {
                    throw new InvalidOperationException(
                        "Current test runner does not support NUnit TestContext - do you need to upgrade ReSharper?");
                }
                return Scenario(nunitTestName);
            }
            catch (NullReferenceException)
            {
                return Scenario("--unnamed--");
            }
        }

        /// <summary>
        /// Annotated method to cause NUnit to call the 'base.BeforeTest'. If you overwrite this be sure to call this on completion
        /// </summary>
        [SetUp]
        public override void BeforeTest()
        {
            base.BeforeTest();
        }

        /// <summary>
        /// Annotated method to cause NUnit to call the base.AfterTest. If you overwrite this be sure to call this on completion
        /// </summary>
        [TearDown]
        public override void AfterTest()
        {
            base.AfterTest();
        }
    }
}
