using System;
using System.Threading.Tasks;
using System.Threading;

namespace TestFirst.Net.Web
{
    public class ThreadWorkerQueue : IWorkQueue
    {
        public ThreadWorkerQueue ()
        {}

        public void Push<T>(Action<T> a,T obj){
            Push (()=>a(obj));
        }

        public void Push(Action a){
            new Thread (() => {
                a.Invoke();
            }).Start();
        }

    }
}

