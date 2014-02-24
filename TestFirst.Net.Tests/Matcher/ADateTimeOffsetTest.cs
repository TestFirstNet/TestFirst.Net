using System;
using NUnit.Framework;
using TestFirst.Net.Matcher;

namespace TestFirst.Net.Test.Matcher
{
    [TestFixture]
    public class ADateTimeOffsetTest: BaseMatcherTest
    {
        [Test]
        public void EqualToTest()
        {
            var dateTime = new DateTimeOffset(new DateTime(1984, 1, 1), TimeSpan.FromHours(11));

            AssertPasses(dateTime, ADateTimeOffset.EqualTo(dateTime));
            AssertPasses(dateTime, ADateTimeOffset.EqualTo(new DateTimeOffset(new DateTime(1984, 1, 1), TimeSpan.FromHours(11))));
            AssertPasses(null, ADateTimeOffset.EqualTo(null));

            AssertFails(null, ADateTimeOffset.EqualTo(dateTime));
            AssertFails(DateTimeOffset.Now, ADateTimeOffset.EqualTo(dateTime));
        }

        [Test]
        public void NullTest()
        {
            AssertPasses(null, ADateTimeOffset.Null());
  
            AssertFails(DateTimeOffset.Now, ADateTimeOffset.Null());
        }

        [Test]
        public void NotNullTest()
        {
            AssertPasses(DateTimeOffset.Now, ADateTimeOffset.NotNull());

            AssertFails(null, ADateTimeOffset.NotNull());
        }

        [Test]
        public void EqualToPlusOrMinusTest()
        {
            var now = DateTimeOffset.Now;
            var matcher = ADateTimeOffset.EqualTo(now).Within(2).Minutes();

            AssertPasses(now.AddMinutes(-2), matcher); 
            AssertPasses(now, matcher);
            AssertPasses(now.AddMinutes(2), matcher);

            AssertFails(now.AddMinutes(-3), matcher);
            AssertFails(now.AddMinutes(3), matcher);
            
        }

        [Test]
        public void EqualToPlusMaxTest()
        {
            var now = DateTimeOffset.Now;
            var matcher = ADateTimeOffset.EqualTo(now).PlusMax(2).Minutes();

            AssertPasses(now, matcher);
            AssertPasses(now.AddMinutes(1), matcher);
            AssertPasses(now.AddMinutes(2), matcher);
            
            AssertFails(now.AddMinutes(-1), matcher);
            AssertFails(now.AddMinutes(-2), matcher);
            AssertFails(now.AddMinutes(-3), matcher);

            AssertFails(now.AddMinutes(3), matcher);
            
             AssertPasses(null,ADateTimeOffset.EqualTo(null).PlusMax(-1).Minutes());
        }

        [Test]
        public void EqualToMinusMaxTest()
        {
            var now = DateTimeOffset.Now;
            var matcher = ADateTimeOffset.EqualTo(now).MinusMax(2).Minutes();

            AssertPasses(now, matcher);
            AssertPasses(now.AddMinutes(-1), matcher);
            AssertPasses(now.AddMinutes(-2), matcher);
            
            AssertFails(now.AddMinutes(1), matcher);
            AssertFails(now.AddMinutes(2), matcher);
            AssertFails(now.AddMinutes(3), matcher);

            AssertFails(now.AddMinutes(-3), matcher);
            
            AssertPasses(null,ADateTimeOffset.EqualTo(null).MinusMax(1).Minutes());
        }

        [Test]
        public void BeforeTest()
        {
            var dateTime = new DateTime(1984,1,1);

            var matcher = ADateTimeOffset.Before(dateTime);

            AssertPasses(dateTime.AddDays(-1), matcher);
            AssertPasses(dateTime.AddMilliseconds(-1), matcher);

            AssertFails(dateTime, matcher);
            AssertFails(dateTime.AddMilliseconds(1), matcher);
            AssertFails(dateTime.AddDays(1), matcher);
        }

        [Test]
        public void AfterTest()
        {
            var dateTime = new DateTime(1984,1,1);

            var matcher = ADateTimeOffset.After(dateTime);

            AssertPasses(dateTime.AddMilliseconds(1), matcher);
            AssertPasses(dateTime.AddDays(1), matcher);
            
            AssertFails(dateTime, matcher);
            AssertFails(dateTime.AddMilliseconds(-1), matcher);
            AssertFails(dateTime.AddDays(-1), matcher);
        }
    }
}
