using TestFirst.Net.Matcher;

namespace TestFirst.Net.Examples 
{
    public class ANotification : PropertyMatcher<TestFirst.Net.Examples.Api.Notification>
    {
        // provide IDE rename and find reference support
        private static readonly TestFirst.Net.Examples.Api.Notification PropertyNames = null;

        public static ANotification With()
        {
            return new ANotification();
        }

        public static IMatcher<TestFirst.Net.Examples.Api.Notification> Null()
        {
            return AnInstance.Null<TestFirst.Net.Examples.Api.Notification>();
        }

        public static IMatcher<TestFirst.Net.Examples.Api.Notification> NotNull()
        {
            return AnInstance.NotNull<TestFirst.Net.Examples.Api.Notification>();
        }

        public static IMatcher<TestFirst.Net.Examples.Api.Notification> Instance(TestFirst.Net.Examples.Api.Notification expect)
        {
            return AnInstance.SameAs(expect);
        }

        public ANotification Id(System.Guid expect)
        {
            Id(AGuid.EqualTo(expect));
            return this;
        }

        public ANotification Id(IMatcher<System.Guid?> matcher) 
        {
            WithProperty(() => PropertyNames.Id, matcher);
            return this;
        }
    }

    public class ANotificationQuery : PropertyMatcher<TestFirst.Net.Examples.Api.Query.NotificationQuery>
    {
        // provide IDE rename and find reference support
        private static readonly TestFirst.Net.Examples.Api.Query.NotificationQuery PropertyNames = null;

        public static ANotificationQuery With()
        {
            return new ANotificationQuery();
        }

        public static IMatcher<TestFirst.Net.Examples.Api.Query.NotificationQuery> Null()
        {
            return AnInstance.Null<TestFirst.Net.Examples.Api.Query.NotificationQuery>();
        }

        public static IMatcher<TestFirst.Net.Examples.Api.Query.NotificationQuery> NotNull()
        {
            return AnInstance.NotNull<TestFirst.Net.Examples.Api.Query.NotificationQuery>();
        }

        public static IMatcher<TestFirst.Net.Examples.Api.Query.NotificationQuery> Instance(TestFirst.Net.Examples.Api.Query.NotificationQuery expect)
        {
            return AnInstance.SameAs(expect);
        }

        public ANotificationQuery AccountId(System.Guid? expect) 
        {
            AccountId(AGuid.EqualTo(expect));
            return this;
        }

        public ANotificationQuery AccountIdNull() 
        {
            AccountId(AGuid.Null());
            return this;
        }

        public ANotificationQuery AccountId(IMatcher<System.Guid?> matcher) 
        {
            WithProperty(() => PropertyNames.AccountId, matcher);
            return this;
        }

        public ANotificationQuery Id(System.Guid? expect) 
        {
            Id(AGuid.EqualTo(expect));
            return this;
        }

        public ANotificationQuery IdNull() 
        {
            Id(AGuid.Null());
            return this;
        }

        public ANotificationQuery Id(IMatcher<System.Guid?> matcher) 
        {
            WithProperty(() => PropertyNames.Id, matcher);
            return this;
        }
    }

    public class AResponse : PropertyMatcher<TestFirst.Net.Examples.Api.Query.NotificationQuery.Response>
    {
        // provide IDE rename and find reference support
        private static readonly TestFirst.Net.Examples.Api.Query.NotificationQuery.Response PropertyNames = null;

        public static AResponse With()
        {
            return new AResponse();
        }

        public static IMatcher<TestFirst.Net.Examples.Api.Query.NotificationQuery.Response> Null()
        {
            return AnInstance.Null<TestFirst.Net.Examples.Api.Query.NotificationQuery.Response>();
        }

        public static IMatcher<TestFirst.Net.Examples.Api.Query.NotificationQuery.Response> NotNull()
        {
            return AnInstance.NotNull<TestFirst.Net.Examples.Api.Query.NotificationQuery.Response>();
        }

        public static IMatcher<TestFirst.Net.Examples.Api.Query.NotificationQuery.Response> Instance(TestFirst.Net.Examples.Api.Query.NotificationQuery.Response expect)
        {
            return AnInstance.SameAs(expect);
        }

        public AResponse ResultNull() 
        {
            Result(AnInstance.EqualTo<TestFirst.Net.Examples.Api.Notification>(null));
            return this;
        }

        public AResponse Result(IMatcher<TestFirst.Net.Examples.Api.Notification> matcher) 
        {
            WithProperty(() => PropertyNames.Result, matcher);
            return this;
        }

        public AResponse ResultsNull() 
        {
            Results(AnInstance.EqualTo<System.Collections.Generic.IEnumerable<TestFirst.Net.Examples.Api.Notification>>(null));
            return this;
        }

        public AResponse Results(IMatcher<System.Collections.Generic.IEnumerable<TestFirst.Net.Examples.Api.Notification>> matcher) 
        {
            WithProperty(() => PropertyNames.Results, matcher);
            return this;
        }
    }
}
