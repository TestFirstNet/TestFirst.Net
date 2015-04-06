namespace TestFirst.Net.Examples.Service.Http
{
    internal interface IRequestProcessor
    {
        void Process(IRequestProvider provider);
    }
}