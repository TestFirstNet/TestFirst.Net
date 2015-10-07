namespace TestFirst.Net.Examples.Net.Http
{
    internal interface IRequestProcessor
    {
        void Process(IRequestProvider provider);
    }
}