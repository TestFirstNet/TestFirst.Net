using System;
using System.Collections.Generic;
using NUnit.Framework;
using TestFirst.Net.Matcher;

namespace TestFirst.Net.Test.Matcher
{
    [TestFixture]
    public class ATimeSpanTest : BaseMatcherTest
    {
        [Test]
        public void EqualToTest()
        {
            var matchers = Matchers(
                ATimeSpan.EqualTo(TimeSpan.FromSeconds(5)),
                ATimeSpan.EqualTo(5).Seconds(),
                ATimeSpan.EqualTo(5000).Milliseconds());

            foreach (var matcher in matchers)
            {
                AssertFails(TimeSpan.FromSeconds(4), matcher);
                AssertFails(TimeSpan.FromSeconds(4), matcher);

                AssertPasses(TimeSpan.FromSeconds(5), matcher);
                AssertPasses(TimeSpan.FromSeconds(5), matcher);
                AssertPasses(TimeSpan.FromMilliseconds(5000), matcher);

                AssertFails(TimeSpan.FromSeconds(6), matcher);
                AssertFails(TimeSpan.FromSeconds(6), matcher);
            }
        }

        [Test]
        public void FluentSingleArgMatcherTest()
        {
            AssertFails(TimeSpan.FromSeconds(4), ATimeSpan.EqualTo(5000).Milliseconds());
            AssertPasses(TimeSpan.FromSeconds(5), ATimeSpan.EqualTo(5000).Milliseconds());
            AssertFails(TimeSpan.FromSeconds(6), ATimeSpan.EqualTo(5000).Milliseconds());
                
            AssertFails(TimeSpan.FromSeconds(4), ATimeSpan.EqualTo(5).Seconds());
            AssertPasses(TimeSpan.FromSeconds(5), ATimeSpan.EqualTo(5).Seconds());
            AssertFails(TimeSpan.FromSeconds(6), ATimeSpan.EqualTo(5).Seconds());

            AssertFails(TimeSpan.FromMinutes(4), ATimeSpan.EqualTo(5).Minutes());
            AssertPasses(TimeSpan.FromMinutes(5), ATimeSpan.EqualTo(5).Minutes());
            AssertFails(TimeSpan.FromMinutes(6), ATimeSpan.EqualTo(5).Minutes());

            AssertFails(TimeSpan.FromHours(4), ATimeSpan.EqualTo(5).Hours());
            AssertPasses(TimeSpan.FromHours(5), ATimeSpan.EqualTo(5).Hours());
            AssertFails(TimeSpan.FromHours(6), ATimeSpan.EqualTo(5).Hours());            

            AssertFails(TimeSpan.FromDays(4), ATimeSpan.EqualTo(5).Days());
            AssertPasses(TimeSpan.FromDays(5), ATimeSpan.EqualTo(5).Days());
            AssertFails(TimeSpan.FromDays(6), ATimeSpan.EqualTo(5).Days());  
        }

        [Test]
        public void FluentMultiArgMatcherTest()
        {
            AssertFails(TimeSpan.FromSeconds(2), ATimeSpan.Between(3000, 5000).Milliseconds());
            AssertPasses(TimeSpan.FromSeconds(4), ATimeSpan.Between(3000, 5000).Milliseconds());
            AssertFails(TimeSpan.FromSeconds(6), ATimeSpan.Between(3000, 5000).Milliseconds());
                
            AssertFails(TimeSpan.FromSeconds(2), ATimeSpan.Between(3, 5).Seconds());
            AssertPasses(TimeSpan.FromSeconds(4), ATimeSpan.Between(3, 5).Seconds());
            AssertFails(TimeSpan.FromSeconds(6), ATimeSpan.Between(3, 5).Seconds());

            AssertFails(TimeSpan.FromMinutes(2), ATimeSpan.Between(3, 5).Minutes());
            AssertPasses(TimeSpan.FromMinutes(4), ATimeSpan.Between(3, 5).Minutes());
            AssertFails(TimeSpan.FromMinutes(6), ATimeSpan.Between(3, 5).Minutes());

            AssertFails(TimeSpan.FromHours(2), ATimeSpan.Between(3, 5).Hours());
            AssertPasses(TimeSpan.FromHours(4), ATimeSpan.Between(3, 5).Hours());
            AssertFails(TimeSpan.FromHours(6), ATimeSpan.Between(3, 5).Hours());            

            AssertFails(TimeSpan.FromDays(2), ATimeSpan.Between(3, 5).Days());
            AssertPasses(TimeSpan.FromDays(4), ATimeSpan.Between(3, 5).Days());
            AssertFails(TimeSpan.FromDays(6), ATimeSpan.Between(3, 5).Days());  
        }

        [Test]
        public void GreaterThanTest()
        {
            var matchers = Matchers(
                ATimeSpan.GreaterThan(TimeSpan.FromSeconds(5)),
                ATimeSpan.GreaterThan(5).Seconds(),
                ATimeSpan.GreaterThan(5000).Milliseconds());

            foreach (var matcher in matchers)
            {
                AssertFails(TimeSpan.FromSeconds(5), matcher);
                AssertPasses(TimeSpan.FromSeconds(6), matcher);
            }
        }

        [Test]
        public void GreaterOrEqualToTest()
        {
            var matchers = Matchers(
                ATimeSpan.GreaterOrEqualTo(TimeSpan.FromSeconds(5)),
                ATimeSpan.GreaterOrEqualTo(5).Seconds(),
                ATimeSpan.GreaterOrEqualTo(5000).Milliseconds());

            foreach (var matcher in matchers)
            {
                AssertFails(TimeSpan.FromSeconds(4), matcher);
                AssertPasses(TimeSpan.FromSeconds(5), matcher);
                AssertPasses(TimeSpan.FromSeconds(6), matcher);
            }
        }

        [Test]
        public void LessThanTest()
        {
            var matchers = Matchers(
                ATimeSpan.LessThan(TimeSpan.FromSeconds(5)),
                ATimeSpan.LessThan(5).Seconds(),
                ATimeSpan.LessThan(5000).Milliseconds());

            foreach (var matcher in matchers)
            {
                AssertPasses(TimeSpan.FromSeconds(0), matcher);

                AssertPasses(TimeSpan.FromSeconds(4), matcher);
                AssertPasses(TimeSpan.FromMilliseconds(4000), matcher);

                AssertFails(TimeSpan.FromSeconds(5), matcher);
                AssertFails(TimeSpan.FromMilliseconds(5000), matcher);

                AssertFails(TimeSpan.FromSeconds(6), matcher); 
            }
        }

        [Test]
        public void LessThanOrEqualToTest()
        {
            var matchers = Matchers(
                ATimeSpan.LessThanOrEqualTo(TimeSpan.FromSeconds(5)),
                ATimeSpan.LessThanOrEqualTo(5).Seconds(),
                ATimeSpan.LessThanOrEqualTo(5000).Milliseconds());

            foreach (var matcher in matchers)
            {
                AssertPasses(TimeSpan.FromSeconds(0), matcher);

                AssertPasses(TimeSpan.FromSeconds(4), matcher);
                AssertPasses(TimeSpan.FromMilliseconds(4000), matcher);

                AssertPasses(TimeSpan.FromSeconds(5), matcher);
                AssertPasses(TimeSpan.FromMilliseconds(5000), matcher);

                AssertFails(TimeSpan.FromSeconds(6), matcher);
                AssertFails(TimeSpan.FromMilliseconds(6000), matcher); 
            }   
        }

        [Test]
        public void BetweenTest()
        {
            var matchers = Matchers(
                ATimeSpan.Between(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(7)),
                ATimeSpan.Between(5, 7).Seconds(),
                ATimeSpan.Between(5000, 7000).Milliseconds());

            foreach (var matcher in matchers)
            {
                AssertFails(TimeSpan.FromSeconds(4), matcher);
                AssertFails(TimeSpan.FromSeconds(5), matcher);
                AssertFails(TimeSpan.FromMilliseconds(5000), matcher);

                AssertPasses(TimeSpan.FromSeconds(6), matcher);
                AssertPasses(TimeSpan.FromMilliseconds(6000), matcher);

                AssertFails(TimeSpan.FromSeconds(7), matcher);
                AssertFails(TimeSpan.FromMilliseconds(7000), matcher);
                AssertFails(TimeSpan.FromSeconds(8), matcher);
            }
        }

        [Test]
        public void BetweenIncludingTest()
        {
            var matchers = Matchers(
                ATimeSpan.BetweenIncluding(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(7)),
                ATimeSpan.BetweenIncluding(5, 7).Seconds(),
                ATimeSpan.BetweenIncluding(5000, 7000).Milliseconds());

            foreach (var matcher in matchers)
            {
                AssertFails(TimeSpan.FromSeconds(4), matcher);
                AssertFails(TimeSpan.FromMilliseconds(4000), matcher);

                AssertPasses(TimeSpan.FromSeconds(5), matcher);
                AssertPasses(TimeSpan.FromMilliseconds(5000), matcher);
                AssertPasses(TimeSpan.FromSeconds(6), matcher);
                AssertPasses(TimeSpan.FromSeconds(7), matcher);
                AssertPasses(TimeSpan.FromMilliseconds(7000), matcher);

                AssertFails(TimeSpan.FromSeconds(8), matcher);
                AssertFails(TimeSpan.FromMilliseconds(8000), matcher);
            }
        }

        [Test]
        public void NotTest()
        {
             var matchers = Matchers(
                ATimeSpan.Not(TimeSpan.FromSeconds(5)),
                ATimeSpan.Not(5).Seconds(),
                ATimeSpan.Not(5000).Milliseconds());

            foreach (var matcher in matchers)
            {
                AssertPasses(TimeSpan.FromSeconds(4), matcher);
                AssertFails(TimeSpan.FromSeconds(5), matcher);
                AssertFails(TimeSpan.FromMilliseconds(5000), matcher);
                AssertPasses(TimeSpan.FromSeconds(6), matcher);
            }
        }

        [Test]
        public void NullTest()
        {
            AssertPasses(null, ATimeSpan.Null());
            AssertFails(TimeSpan.FromSeconds(0), ATimeSpan.Null());
            AssertFails(TimeSpan.FromSeconds(1), ATimeSpan.Null());
        }

        [Test]
        public void NotNullTest()
        {
            AssertPasses(TimeSpan.FromSeconds(0), ATimeSpan.NotNull());
            AssertPasses(TimeSpan.FromSeconds(1), ATimeSpan.NotNull());
            AssertFails(null, ATimeSpan.NotNull());
        }

        private IList<IMatcher<T>> Matchers<T>(params IMatcher<T>[] matchers)
        {
            return new List<IMatcher<T>>(matchers);
        }
    }
}
