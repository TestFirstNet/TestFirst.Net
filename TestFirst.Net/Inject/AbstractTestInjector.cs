using System;
using System.Collections.Generic;
using TestFirst.Net.Inject;
using Inject;
using System.Threading.Tasks;
using System.Security.Policy;
using System.Collections;

namespace TestFirst.Net.Inject
{
    /// <summary>
    /// Performs all the default injection. Also disposes of disposables
    /// </summary>
    public abstract class AbstractTestInjector : ITestInjector,IDisposable
    {
        private List<IInjectGuard> _guards;

        private List<ITestInjector> _childInjectors;

        private List<ITestInjector> _postInjectors;

        private volatile bool _disposed;

        public AbstractTestInjector(){
        }

        public void InjectDependencies<T>(T instance)
        {
            CheckNotDisposed ();

            if (_guards != null) {
                foreach (var guard in _guards) {
                    if (!guard.Inject (instance)) {
                        return;
                    }
                }
            }

            OnInject(instance);

            if (_childInjectors != null) {
                foreach (var injector in _childInjectors) {
                    injector.InjectDependencies(instance);
                }
            }

            if (_postInjectors != null) {
                foreach (var injector in _postInjectors) {
                    injector.InjectDependencies(instance);
                }
            }

            AfterInject (instance);
        }

        protected abstract void OnInject (object instance);

        protected virtual void AfterInject (object instance){
            //sub classes to implement
        }

        public void RemoveInjector(ITestInjector injector){
            RemoveChildInjector (injector);
            RemovePostInjector (injector);
        }

        public void AddGuard(IInjectGuard guard){
            CheckNotDisposed ();
            if (guard == null) {
                return;
            }
            if (_guards == null) {
                _guards = new List<IInjectGuard> ();
            }
            _guards.Add (guard);
        }

        public void AddChildInjector(ITestInjector injector){
            CheckNotDisposed ();
            if (injector == null) {
                return;
            }
            if (_childInjectors == null) {
                _childInjectors = new List<ITestInjector> ();
            }
            _childInjectors.Add (injector);
        }

        public void RemoveChildInjector(ITestInjector injector){
            if (injector == null) {
                return;
            }
            if (_childInjectors != null) {
                _childInjectors.Remove (injector);
            }
        }

        public void AddPostInjector(ITestInjector injector){
            CheckNotDisposed ();
            if (injector == null) {
                return;
            }
            if (_postInjectors == null) {
                _postInjectors = new List<ITestInjector> ();
            }
            _postInjectors.Add (injector);
        }

        public void RemovePostInjector(ITestInjector injector){
            if (injector == null) {
                return;
            }
            if (_postInjectors != null) {
                _postInjectors.Remove (injector);
            }
        }

        protected void CheckNotDisposed(){
            if (_disposed) {
                throw new InvalidOperationException ("Injector is disposed");
            }
        }

        public void Dispose(){
            if (!_disposed) {
                _disposed = true;
                DisposeInReverseAndClear (_guards);
                DisposeInReverseAndClear (_childInjectors);
                DisposeInReverseAndClear (_postInjectors);
            }
        }

        private void DisposeInReverseAndClear(IList items){
            if (items != null) {
                for (var i = items.Count; i >= 0; i--) {
                    var disposable = items [i] as IDisposable;
                    if (disposable != null) {
                        try {
                            disposable.Dispose();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error disposing " + disposable.GetType().FullName);
                            Console.WriteLine(e);
                        }
                    }
                }
                items.Clear ();
            }
        }

        
    }
}
