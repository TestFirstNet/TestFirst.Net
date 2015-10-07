using System.Collections.Generic;
using NUnit.Framework;
using Shouldly;

namespace TestFirst.Net.Test.Matcher
{
    [TestFixture]
    public class AbstractMatcherTest
    {
        [Test]
        public void ActualIsPassedToSubclass()
        {
            var test = new TestMatcher<string>();

            var matched = test.Matches("abc");

            test.CapturedActual.Count.ShouldBe(1);
            test.CapturedActual.ShouldContain("abc");
            matched.ShouldBe(true);
        }

        [Test]
        public void FailsWhenSubclassMatchFails()
        {
            var test = new TestMatcher<string>();
            test.MatchPasses = false;

            var matched = test.Matches("SomeValue");

            matched.ShouldBe(false);
        }

        [Test]
        public void NullDiagnosticsIsProvidedByDefault()
        {
            var test = new TestMatcher<string>();

            test.Matches("SomeValue");

            test.CapturedDiagnostics.Count.ShouldBe(1);
            test.CapturedDiagnostics[0].IsNull.ShouldBe(true);
        }

        [Test]
        public void ProvidedDiagnosticsIsPassedOn()
        {
            var test = new TestMatcher<string>();
            var diagnostics = Mock<IMatchDiagnostics>();

            test.Matches("SomeValue", diagnostics);

            test.CapturedDiagnostics.Count.ShouldBe(1);
            test.CapturedDiagnostics[0].ShouldBeSameAs(diagnostics);
        }

        [Test]
        public void FailsOnMismatchedSimpleTypeWithoutCallingSubclass()
        {
            var test = new TestMatcher<string>();
            var diagnostics = Mock<IMatchDiagnostics>();

            var matched = ((IMatcher)test).Matches(123, diagnostics);

            matched.ShouldBe(false);
            test.CapturedActual.Count.ShouldBe(0);
        }

        [Test]
        public void PassesGenericTypeToSubClass()
        {
            var test = new TestMatcher<List<string>>();
            var diagnostics = Mock<IMatchDiagnostics>();

            List<string> actual = new List<string>();
            var matched = test.Matches(actual, diagnostics);

            matched.ShouldBe(true);
            test.CapturedActual.Count.ShouldBe(1);
            test.CapturedActual[0].ShouldBe(actual);
        }

        [Test]
        public void FailsOnMismatchedGenericTypeWithoutCallingSubclass()
        {
            var test = new TestMatcher<List<string>>();
            var diagnostics = Mock<IMatchDiagnostics>();

            var matched = ((IMatcher)test).Matches(new List<int>(), diagnostics);

            matched.ShouldBe(false);
            test.CapturedActual.Count.ShouldBe(0);
        }

        [Test]
        public void AllowsSubclassTypesToBeMatched()
        {
            var test = new TestMatcher<MyTestType>();
            
            var matched = test.Matches(new MyTestTypeSubClass());

            matched.ShouldBe(true);
            test.CapturedActual.Count.ShouldBe(1);
        }

        [Test]
        public void AllowsSubclassGenericTypesToBeMatched()
        {
            var test = new TestMatcher<IEnumerable<string>>();

            var matched = test.Matches(new List<string>());

            matched.ShouldBe(true);
            test.CapturedActual.Count.ShouldBe(1);
        }

        [Test]
        public void PassesWhenNullAndNullsAllowed()
        {
            var test = new TestMatcher<string>(true /*allow nulls*/);

            var matched = test.Matches(null);

            matched.ShouldBe(true);
            test.CapturedActual.Count.ShouldBe(1);
        }

        [Test]
        public void FailsWhenNullAndNullsAreNotAllowed()
        {
            var test = new TestMatcher<string>(false/*don't allow nulls*/);

            var matched = test.Matches(null);

            matched.ShouldBe(false);
            test.CapturedActual.Count.ShouldBe(0);
        }

        [Test]
        public void FailsWhenActualIsNullAndTypeIsNotNullable()
        {
            var test = new TestMatcher<int>(true/*try to allow nulls which is ignored*/);

            var matched = test.Matches((int?)null);

            matched.ShouldBe(false);
            test.CapturedActual.Count.ShouldBe(0);
        }

        private T Mock<T>() where T : class
        {
            return NSubstitute.Substitute.For<T>();
        }

        private class MyTestType
        {
        }

        private class MyTestTypeSubClass : MyTestType
        {
        }

        private class TestMatcher<T> : AbstractMatcher<T>
        {
            public TestMatcher(bool allowNulls = true)
                : base(allowNulls)
            {
                MatchPasses = true;
                CapturedActual = new List<T>();
                CapturedDiagnostics = new List<IMatchDiagnostics>();
            }

            public List<T> CapturedActual { get; private set; }
            public List<IMatchDiagnostics> CapturedDiagnostics { get; private set; }

            public bool MatchPasses { private get; set; }

            public override bool Matches(T actual, IMatchDiagnostics diag)
            {
                CapturedActual.Add(actual);
                CapturedDiagnostics.Add(diag);
                return MatchPasses;
            }
        }
    }
}
