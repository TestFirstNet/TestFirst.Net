namespace TestFirst.Net
{
    /// <summary>
    /// Used to build state in the givens
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBuilder<out T>
    {
        T Build();
    }
}