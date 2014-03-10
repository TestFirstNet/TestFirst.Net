using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TestFirst.Net.Lang;

namespace TestFirst.Net.Random
{
    [Obsolete("use TestFirst.Net.Rand.RandomFiller instead")]
    public static class RandomFiller
    {
        public static TestFirst.Net.Rand.RandomFiller.Builder With()
        {
            return new TestFirst.Net.Rand.RandomFiller.Builder();
        }

        public class Builder : TestFirst.Net.Rand.RandomFiller.Builder
        {

        }
    }
}