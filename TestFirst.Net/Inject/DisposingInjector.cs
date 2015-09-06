using System;
using System.Collections.Generic;
using Inject;

namespace TestFirst.Net.Inject
{
    /// <summary>
    /// Collect all disposables and dispose at end of scenario
    /// </summary>
    public class DisposingInjector : ITestInjector,IDisposable
    {
        private readonly IList<IDisposable> m_disposables = new List<IDisposable>();

        public void InjectDependencies<T>(T instance)
        {
            AddDisposable (instance);
        }

        protected void AddDisposable(object instance){
            var disposable = instance as IDisposable;
            if(disposable != null)
            {
                m_disposables.Add(disposable);
            }
        }

        public void Dispose()
        {
            //dispose in reverse order added
            for (var i = m_disposables.Count; i >= 0; i--)
            {
                var disposable = m_disposables[i];
                try
                {
                    disposable.Dispose();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error disposing " + disposable.GetType().FullName);
                    Console.WriteLine(e);
                }
            }
            m_disposables.Clear();
        }
    }
}
