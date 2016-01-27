using System;
using System.Collections.Generic;
using System.Linq;

namespace TestFirst.Net
{
    /// <summary>
    /// Collect all disposables and dispose at end of scenario
    /// </summary>
    public class DisposingStepArgInjector : IStepArgDependencyInjector
    {
        private readonly IList<IDisposable> m_disposables = new List<IDisposable>();
             
        public void Dispose()
        {
            var reversed = m_disposables.Reverse();
            foreach (var disposable in reversed)
            {
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

        public virtual void InjectDependencies<T>(T instance)
        {
            var disposable = instance as IDisposable;
            if (disposable != null && !m_disposables.Contains(disposable))
            {
                m_disposables.Add(disposable);
            }
        }
    }
}
