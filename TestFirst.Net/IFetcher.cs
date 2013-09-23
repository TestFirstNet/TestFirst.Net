namespace TestFirst.Net
{
    /// <summary>
    /// Used to fetch an object from a DB, Network etc upon which matchers can be invoked against
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IFetcher<out T>
    {
        T Fetch();
    }
}