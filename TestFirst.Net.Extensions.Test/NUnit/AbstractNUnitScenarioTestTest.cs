using NUnit.Framework;
using TestFirst.Net.Extensions.NUnit;

namespace TestFirst.Net.Extensions.Test.NUnit
{
    [TestFixture]
    public class AbstractNUnitScenarioTestTest : AbstractNUnitScenarioTest
    {
        [Test]
        public void ScenarioTitleIsSetFromTestMethodNameTest()
        {
            var scenario = Scenario();
            
            scenario.GivenNothing().WhenNothing().Then(() => { /* just to pass scenario */ });

            Assert.AreEqual("ScenarioTitleIsSetFromTestMethodNameTest", scenario.Title);
        }
    }
}
