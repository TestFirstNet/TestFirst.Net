using System;
using NUnit.Framework;
using TestFirst.Net.Matcher;

namespace TestFirst.Net.Test.Matcher
{
    [TestFixture]
    public class ADateTimeTest: BaseMatcherTest
    {
        [Test]
        public void EqualToTest()
        {
            var dateTime = new DateTime(1984,1,1);

            AssertPasses(dateTime, ADateTime.EqualTo(dateTime));
            AssertPasses(dateTime, ADateTime.EqualTo(new DateTime(1984,1,1)));
            AssertPasses(null, ADateTime.EqualTo(null));

            AssertFails(null, ADateTime.EqualTo(dateTime));
            AssertFails(DateTime.Now, ADateTime.EqualTo(dateTime));
        }

        [Test]
        public void NullTest()
        {
            AssertPasses(null, ADateTime.Null());
  
            AssertFails(DateTime.Now, ADateTime.Null());
        }

        [Test]
        public void NotNullTest()
        {
            AssertPasses(DateTime.Now, ADateTime.NotNull());

            AssertFails(null, ADateTime.NotNull());
        }

        [Test]
        public void EqualToPlusOrMinusTest()
        {
            var now = DateTime.Now;
            var matcher = ADateTime.EqualTo(now).Within(2).Minutes();

            AssertPasses(now.AddMinutes(-2), matcher); 
            AssertPasses(now, matcher);
            AssertPasses(now.AddMinutes(2), matcher);

            AssertFails(now.AddMinutes(-3), matcher);
            AssertFails(now.AddMinutes(3), matcher);
            
        }

        [Test]
        public void EqualToPlusMaxTest()
        {
            var now = DateTime.Now;
            var matcher = ADateTime.EqualTo(now).PlusMax(2).Minutes();

            AssertPasses(now, matcher);
            AssertPasses(now.AddMinutes(1), matcher);
            AssertPasses(now.AddMinutes(2), matcher);
            
            AssertFails(now.AddMinutes(-1), matcher);
            AssertFails(now.AddMinutes(-2), matcher);
            AssertFails(now.AddMinutes(-3), matcher);

            AssertFails(now.AddMinutes(3), matcher);
            
             AssertPasses(null,ADateTime.EqualTo(null).PlusMax(-1).Minutes());
        }

        [Test]
        public void EqualToMinusMaxTest()
        {
            var now = DateTime.Now;
            var matcher = ADateTime.EqualTo(now).MinusMax(2).Minutes();

            AssertPasses(now, matcher);
            AssertPasses(now.AddMinutes(-1), matcher);
            AssertPasses(now.AddMinutes(-2), matcher);
            
            AssertFails(now.AddMinutes(1), matcher);
            AssertFails(now.AddMinutes(2), matcher);
            AssertFails(now.AddMinutes(3), matcher);

            AssertFails(now.AddMinutes(-3), matcher);
            
            AssertPasses(null,ADateTime.EqualTo(null).MinusMax(1).Minutes());
        }

        [Test]
        public void BeforeTest()
        {
            var dateTime = new DateTime(1984,1,1);

            var matcher = ADateTime.Before(dateTime);

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

            var matcher = ADateTime.After(dateTime);

            AssertPasses(dateTime.AddMilliseconds(1), matcher);
            AssertPasses(dateTime.AddDays(1), matcher);
            
            AssertFails(dateTime, matcher);
            AssertFails(dateTime.AddMilliseconds(-1), matcher);
            AssertFails(dateTime.AddDays(-1), matcher);
        }
    }
}
