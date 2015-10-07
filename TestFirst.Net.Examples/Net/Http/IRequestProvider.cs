namespace TestFirst.Net.Examples.Net.Http
{
    internal interface IRequestProvider
    {
        /// <summary>
        /// Grab the request to process. This can be called multiple times to return the same request. It is likely only
        /// the first call incurs any overhead to setup the request
        /// </summary>
        /// <returns>The request context</returns>
        RequestContext Take();
    }
}