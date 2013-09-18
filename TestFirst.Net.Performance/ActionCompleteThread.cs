using System;
using System.Threading;

namespace TestFirst.Net.Performance
{
    /// <summary>
    /// Provide ability to determine if an action has run
    /// </summary>
    internal class ActionCompleteThread
    {
        private readonly Action m_action;
        private readonly Thread m_thread;
        private volatile bool m_completed;

        internal bool Completed
        {
            get { return m_completed; }
        }

        internal ActionCompleteThread(Action action)
        {
            m_action = () => 
            {
                try
                {  
                    action.Invoke();
                }
                finally
                {
                    m_completed = true;
                }
            };
            m_thread = new Thread(m_action.Invoke);
        }

        internal ActionCompleteThread Where(Action<Thread> threadMutator)
        {
            threadMutator.Invoke(m_thread);
            return this;
        }

        public void Start()
        {                
            m_thread.Start();
        }

        public void Abort()
        {
            m_thread.Abort();
        }   
    }
}