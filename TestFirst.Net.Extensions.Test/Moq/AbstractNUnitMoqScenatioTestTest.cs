using System;
using Moq;
using NUnit.Framework;
using TestFirst.Net.Extensions.Moq;
using TestFirst.Net.Matcher;

namespace TestFirst.Net.Extensions.Test.Moq
{
    [TestFixture]
    public class AbstractNUnitMoqScenatioTestTest : AbstractNUnitMoqScenarioTest
    {
        [Test]
        public void AMockPassesTest()
        {
            ISpeaker speaker;
            String said;

            Scenario("AMockTest")
                .Given(speaker = AMock<ISpeaker>()
                    .WhereMethod(t=>t.Says())
                    .Returns("DoDah")
                    .Instance)
                .When(said = speaker.Says())
                .Then(ExpectThat(said), Is(AString.EqualTo("DoDah")));
        }

        [Test]
        public void AMockFailsVerifyAllTest()
        {
            Scenario("AMockTest")                
                .Given(AMock<ISpeaker>()
                    .WhereMethod(t=>t.Says())
                    .Returns("DoDah")
                    .Instance)
                .WhenNothing()
                .Then(()=>{ /*Scenario passes but verify should fail*/});

            MockException thrown = null;
            try
            {
                //force scenario completion and assertions
                AfterTest();
            }
            catch (Exception e)
            {

                thrown = e.InnerException as MockException;
            }

            AnException.With()
                .Type("Moq.MockVerificationException")
                .Message(AString.Containing("ISpeaker t => t.Says()"))
                .AssertMatch(thrown);
        }

        public interface ISpeaker
        {
            String Says();
        }
    }
}
