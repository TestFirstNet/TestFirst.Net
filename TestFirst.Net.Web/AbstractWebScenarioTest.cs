using System;
using System.Threading;

namespace TestFirst.Net.Web
{
    public abstract class AbstractWebScenarioTest : AbstractScenarioTest
    {
        public AbstractWebScenarioTest ()
        {
        }

        protected Action Pause(){
            var start = DateTime.Now;
            var max = TimeSpan.FromMinutes(2);
            System.Console.Write("Test Paused.Hit any alphanumeric + ENTER to continue:");
            var result = System.Console.ReadLine();
            while(result == null || result.Trim().Length ==0){

                Thread.Sleep(1000);
                result = System.Console.ReadLine();
                Console.ReadKey();
                if((DateTime.Now - start) >= max){
                    break;
                }
            }

            System.Console.WriteLine("Exiting");

            return () => {};
        }

        protected Action WaitForMs(int ms){
            return ()=>{
                Thread.Sleep(ms);
            };
        }
    }
}

