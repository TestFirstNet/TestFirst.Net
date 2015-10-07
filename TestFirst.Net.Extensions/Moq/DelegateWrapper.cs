using System;

namespace TestFirst.Net.Extensions.Moq
{
    internal class DelegateWrapper
    {
        private Delegate m_delegate;

        public Delegate Delegate
        {
            get { return m_delegate; }
        }

        public void Add(Delegate d)
        {
            m_delegate = Delegate.Combine(m_delegate, d);
        }

        public void Remove(Delegate d)
        {
            m_delegate = Delegate.Remove(m_delegate, d);
        }

        public void Invoke(params object[] args)
        {
            if (m_delegate != null)
                m_delegate.DynamicInvoke(args);
        }
    }
}
