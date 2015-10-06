using System;

namespace TestFirst.Net.Random
{
    [Obsolete("use TestFirst.Net.Rand.RandomFiller instead")]
    public static class RandomFiller
    {
        public static Rand.RandomFiller.Builder With()
        {
            return new Rand.RandomFiller.Builder();
        }

        public class Builder : Rand.RandomFiller.Builder
        {
        }
    }
}