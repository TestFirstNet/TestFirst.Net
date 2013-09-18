using System;
using TestFirst.Net.Matcher;

namespace TestFirst.Net.Examples.Api
{
    public class ANotification : PropertyMatcher<Notification>
    {
        //allow us to access refactor safe proeprty names without resorting to magic strings
        private static readonly Notification PropertyNames = null;

        public static ANotification With()
        {
            return new ANotification();
        }

        public static  IMatcher<Notification> Instance(Notification val)
        {
            return AnInstance.SameAs(val);
        }

        public ANotification Id(Guid? val)
        {
            Id(AGuid.EqualTo(val));
            return this;
        }
        
        public ANotification Id(IMatcher<Guid?> matcher)
        {
            WithProperty(() => PropertyNames.Id, matcher);
            return this;
        }
    }
}
