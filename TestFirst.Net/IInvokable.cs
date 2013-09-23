namespace TestFirst.Net
{
    public interface IInvokable
    {
        void Invoke();
    }

    public interface IInvokable<T>
    {
        T Invoke();
    }
}
