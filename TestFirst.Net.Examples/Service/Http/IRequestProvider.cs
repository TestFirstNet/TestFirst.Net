namespace TestFirst.Net.Examples.Service.Http
{
    internal interface IRequestProvider
    {
        /// <summary>
        /// Grab the request to process. This can be called multiple times to retrn the same request. It is likely only
        /// the first call incurs any overhead to setup the request
        /// </summary>
        Request Take();
    }
}