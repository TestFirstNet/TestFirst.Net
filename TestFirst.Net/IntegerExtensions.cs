using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestFirst.Net
{
    public static class IntegerExtensions
    {
        public static TimeSpan Milliseconds(this int i)
        {
            return TimeSpan.FromMilliseconds(i);
        }

        public static TimeSpan Seconds(this int i)
        {
            return TimeSpan.FromSeconds(i);
        }

        public static TimeSpan Minutes(this int i)
        {
            return TimeSpan.FromMinutes(i);
        }
        
        public static TimeSpan Hours(this int i)
        {
            return TimeSpan.FromHours(i);
        }

        public static TimeSpan Days(this int i)
        {
            return TimeSpan.FromDays(i);
        }
    }
}
