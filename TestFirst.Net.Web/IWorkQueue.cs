using System;
using System.Threading.Tasks;

namespace TestFirst.Net.Web
{
    public interface IWorkQueue
    {
        void Push(Action a);
        void Push<T>(Action<T> a,T obj);

    }
}

