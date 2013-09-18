using System;
using System.Collections.Generic;

namespace TestFirst.Net
{
    /// <summary>
    /// Collect all disposables and dispose at end of scenario
    /// </summary>
    public class DisposingStepArgInjector : IStepArgDependencyInjector, IDisposable
    {
        private readonly IList<IDisposable> m_disposables = new List<IDisposable>();
             
        public void Dispose()
        {
            foreach (var disposable in m_disposables)
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

        virtual public void InjectDependencies<T>(T instance)
        {
            if( instance is IDisposable)
            {
                m_disposables.Add(instance as IDisposable);
            }
        }
    }
}
