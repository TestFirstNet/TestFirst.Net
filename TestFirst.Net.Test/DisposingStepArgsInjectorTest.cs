using System;
using NSubstitute;
using NUnit.Framework;

namespace TestFirst.Net.Test
{
    [TestFixture]
    public class DisposingStepArgsInjectorTest
    {
        [Test]
        public void DisposableIsDisposed()
        {
            var injector = new DisposingStepArgInjector();
            var disposable  = Mock<IDisposable>();

            injector.InjectDependencies(disposable);

            injector.Dispose();

            disposable.Received(1).Dispose();
        }

        [Test]
        public void DisposableIsRegisteredOnlyOnce()
        {
            var injector = new DisposingStepArgInjector();
            var disposable = Mock<IDisposable>();

            injector.InjectDependencies(disposable);
            injector.InjectDependencies(disposable);

            injector.Dispose();

            disposable.Received(1).Dispose();
        }

        [Test]
        public void DisposablesAreDisposedInReverseOrderTheyWereAdded()
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
        public void AllAreDisposedEvenIfDisposableThrowsException()
        {
            var injector = new DisposingStepArgInjector();
            var disposable1 = Mock<IDisposable>();
            var disposable2 = Mock<IDisposable>();
            disposable2.When(_ => _.Dispose()).Do(_ => { throw new Exception(); });
            var disposable3 = Mock<IDisposable>();

            injector.InjectDependencies(disposable1);
            injector.InjectDependencies(disposable2);
            injector.InjectDependencies(disposable3);

            injector.Dispose();
        
            disposable1.Received().Dispose();
            disposable2.Received().Dispose();
            disposable3.Received().Dispose();
        }

        [Test]
        public void NullsIgnoredAndDontThrowError()
        {
            var injector = new DisposingStepArgInjector();
            injector.InjectDependencies((IDisposable)null);
            injector.Dispose();
        }

        [Test]
        public void NonDisposabledIgnored()
        {
            var injector = new DisposingStepArgInjector();
            injector.InjectDependencies("mystring");
            injector.Dispose();
        }

        private T Mock<T>() where T : class 
        {
            return Substitute.For<T>();
        }
    }
}
