namespace TestFirst.Net
{
    /// <summary>
    /// Used to build state in the givens
    /// </summary>
    /// <typeparam name="T">The type of the built object</typeparam>
    public interface IBuilder<out T>
    {
        T Build();
    }
}