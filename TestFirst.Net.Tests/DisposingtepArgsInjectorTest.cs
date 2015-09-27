using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NSubstitute;

namespace TestFirst.Net.Test
{
    [TestFixture]
    public class DisposingtepArgsInjectorTest
    {
        [Test]
        public void disposable_is_disposed()
        {
            var injector = new DisposingStepArgInjector();
            var disposable  = Mock<IDisposable>();

            injector.InjectDependencies(disposable);

            injector.Dispose();

            disposable.Received(1).Dispose();
        }

        [Test]
        public void disposable_is_registered_only_once()
        {
            var injector = new DisposingStepArgInjector();
            var disposable = Mock<IDisposable>();

            injector.InjectDependencies(disposable);
            injector.InjectDependencies(disposable);

            injector.Dispose();

            disposable.Received(1).Dispose();
        }

        [Test]
        public void disposables_are_disposed_in_reverse_order_they_were_added()
        {
            var injector = new DisposingStepArgInjector();
            var disposable1 = Mock<IDisposable>();
            var disposable2 = Mock<IDisposable>();

            injector.InjectDependencies(disposable1);
            injector.InjectDependencies(disposable2);

            injector.Dispose();

            Received.InOrder(() =>
            {
                disposable2.Dispose();
                disposable1.Dispose();
            });
        }

        [Test]
        public void nulls_ignored_and_dont_throw_error()
        {
            var injector = new DisposingStepArgInjector();
            injector.InjectDependencies((IDisposable)null);
            injector.Dispose();
        }

        [Test]
        public void non_disposabled_ignored()
        {
            var injector = new DisposingStepArgInjector();
            injector.InjectDependencies("mystring");
            injector.Dispose();
        }

        private T Mock<T>() where T:class 
        {
            return NSubstitute.Substitute.For<T>();
        }
    }
}
