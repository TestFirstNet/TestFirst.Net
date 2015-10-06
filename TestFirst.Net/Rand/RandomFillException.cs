using System;

namespace TestFirst.Net.Rand
{
    public class RandomFillException : Exception
    {
        public RandomFillException(string msg) 
            : base(msg)
        {
        }

        public RandomFillException(string msg, Exception e)
            : base(msg, e)
        {
        }
    }
}
