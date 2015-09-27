using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Shouldly;

namespace TestFirst.Net.Test.Matcher
{
    [TestFixture]
    public class AbstractMatcherTest
    {
        [Test]
        public void actual_is_passed_to_subclass()
        {
            var test = new TestMatcher<string>();

            var matched = test.Matches("abc");

            test.CapturedActual.Count.ShouldBe(1);
            test.CapturedActual.ShouldContain("abc");
            matched.ShouldBe(true);
        }

        [Test]
        public void fails_when_subclass_match_fails()
        {
            var test = new TestMatcher<string>();
            test.MatchPasses = false;

            var matched = test.Matches("SomeValue");

            matched.ShouldBe(false);
        }

        [Test]
        public void null_diagnostics_is_provided_by_default()
        {
            var test = new TestMatcher<string>();

            test.Matches("SomeValue");

            test.CapturedDiagnostics.Count.ShouldBe(1);
            test.CapturedDiagnostics[0].IsNull.ShouldBe(true);
        }

        [Test]
        public void provided_diagnostics_is_passed_on()
        {
            var test = new TestMatcher<string>();
            var diagnostics = Mock<IMatchDiagnostics>();

            test.Matches("SomeValue", diagnostics);

            test.CapturedDiagnostics.Count.ShouldBe(1);
            test.CapturedDiagnostics[0].ShouldBeSameAs(diagnostics);
        }

        [Test]
        public void fails_on_mismatched_simple_type_without_calling_subclass()
        {
            var test = new TestMatcher<string>();
            var diagnostics = Mock<IMatchDiagnostics>();

            var matched = ((IMatcher)test).Matches(123, diagnostics);

            matched.ShouldBe(false);
            test.CapturedActual.Count.ShouldBe(0);
        }

        
        [Test]
        public void passes_generic_type_to_sub_class()
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
        public void fails_on_mismatched_generic_type_without_calling_subclass()
        {
            var test = new TestMatcher<List<string>>();
            var diagnostics = Mock<IMatchDiagnostics>();

            var matched = ((IMatcher)test).Matches(new List<int>(), diagnostics);

            matched.ShouldBe(false);
            test.CapturedActual.Count.ShouldBe(0);
        }

        [Test]
        public void allows_subclass_types_to_be_matched()
        {
            var test = new TestMatcher<MyTestType>();
            
            var matched = test.Matches(new MyTestTypeSubClass());

            matched.ShouldBe(true);
            test.CapturedActual.Count.ShouldBe(1);
        }

        [Test]
        public void allows_subclass_generic_types_to_be_matched()
        {
            var test = new TestMatcher<IEnumerable<string>>();

            var matched = test.Matches(new List<string>());

            matched.ShouldBe(true);
            test.CapturedActual.Count.ShouldBe(1);
        }

        [Test]
        public void passes_when_null_and_nulls_allowed()
        {
            var test = new TestMatcher<string>(true /*allow nulls*/);

            var matched = test.Matches(null);

            matched.ShouldBe(true);
            test.CapturedActual.Count.ShouldBe(1);
        }

        [Test]
        public void fails_when_null_and_nulls_are_not_allowed()
        {
            var test = new TestMatcher<string>(false/*don't allow nulls*/);

            var matched = test.Matches(null);

            matched.ShouldBe(false);
            test.CapturedActual.Count.ShouldBe(0);
        }

        [Test]
        public void fails_when_actual_is_null_and_type_is_not_nullable()
        {
            var test = new TestMatcher<int>(true/*try to allow nulls which is ignored*/);

            var matched = test.Matches(((int?)null));

            matched.ShouldBe(false);
            test.CapturedActual.Count.ShouldBe(0);
        }


        private T Mock<T>() where T:class
        {
            return NSubstitute.Substitute.For<T>();
        }

        class MyTestType
        {

        }
        class MyTestTypeSubClass : MyTestType
        {

        }
        class TestMatcher<T> : AbstractMatcher<T>
        {
            public List<T> CapturedActual = new List<T>();
            public List<IMatchDiagnostics> CapturedDiagnostics = new List<IMatchDiagnostics>();
            public bool MatchPasses = true;

            public TestMatcher(bool allowNulls=true):base(allowNulls)
            {
            }

            public override bool Matches(T actual, IMatchDiagnostics diag)
            {
                CapturedActual.Add(actual);
                CapturedDiagnostics.Add(diag);
                return MatchPasses;
            }
        }
    }
}
