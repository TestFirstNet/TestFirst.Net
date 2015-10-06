namespace TestFirst.Net
{
    /// <summary>
    /// Builder which simply returns the instance provided at construction
    /// </summary>
    /// <typeparam name="T">The type of the instance to return</typeparam>
    public class PassInstanceBuilder<T> : IBuilder<T>
    {
        private readonly T m_instance;

        public PassInstanceBuilder(T instance)
        {
            m_instance = instance;
        }

        public static PassInstanceBuilder<TInstance> With<TInstance>(TInstance instance)
        {
            return new PassInstanceBuilder<TInstance>(instance);
        }

        public T Build()
        {
            return m_instance;
        }
    }
}
