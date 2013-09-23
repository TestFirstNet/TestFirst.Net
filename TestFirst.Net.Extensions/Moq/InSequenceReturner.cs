namespace TestFirst.Net.Extensions.Moq
{
    internal class InSequenceReturner<TReturns>
    {
        private readonly TReturns[] m_values;
        private int m_index;

        public InSequenceReturner(TReturns[] values)
        {
            m_values = values;
            m_index = 0;
        }

        public TReturns NextValue()
        {
            if (m_index < m_values.Length)
            {
                return m_values[m_index++];
            }
            throw new AssertionFailedException("Tried to return a sequence value more than " + m_values.Length + " times");
        }
    }
}