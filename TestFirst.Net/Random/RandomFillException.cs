using System;

namespace TestFirst.Net.Random
{
    public class RandomFillException : Exception
    {
        public RandomFillException(String msg):base(msg)
        {
        }

        public RandomFillException(String msg,Exception e):base(msg,e)
        {
        }
    }

}
