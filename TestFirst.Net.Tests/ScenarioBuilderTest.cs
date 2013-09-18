using NUnit.Framework;
using TestFirst.Net.Matcher;

namespace TestFirst.Net.Test
{
    [TestFixture]
    public class ScenarioBuilderTest : BaseScenarioTest
    {

        [Test]
        public void GivenBuilder_whenNothing_thenMatcher_forBuildResult()
        {
            //passing
            string val;
            Scenario()
                .Given(val = A(Build("foo")))
                .Then(val, AString.EqualTo("foo"));
        }


        IBuilder<T> Build<T>(T instance)
        {
            return new ReturnInstanceBuilderr<T>(instance);
        }

        private class ReturnInstanceBuilderr<T> : IBuilder<T>
        {
            private readonly T m_instance;

            public ReturnInstanceBuilderr(T instance)
            {
                m_instance = instance;
            }

            public T Build()
            {
                return m_instance;
            }
        }
    }
}
