using System;
using System.Collections.Generic;
using TestFirst.Net.Matcher;

namespace TestFirst.Net.Examples {

    public class ANotification : PropertyMatcher<Api.Notification>{

        // provide IDE rename and find reference support
        private static readonly Api.Notification PropertyNames = null;


        public static ANotification With(){
                return new ANotification();
        }

        public static IMatcher<Api.Notification> Null(){
                return AnInstance.Null<Api.Notification>();
        }

        public static IMatcher<Api.Notification> NotNull(){
                return AnInstance.NotNull<Api.Notification>();
        }

        public static IMatcher<Api.Notification> Instance(Api.Notification expect){
                return AnInstance.SameAs(expect);
        }

        public ANotification Id(Guid expect) {
            Id(AGuid.EqualTo(expect));
            return this;
        }

        public ANotification Id(IMatcher<Guid?> matcher) {
            WithProperty(()=>PropertyNames.Id,matcher);
            return this;
        }
    }
}

namespace TestFirst.Net.Examples {

    public class ANotificationQuery : PropertyMatcher<Api.Query.NotificationQuery>{

        // provide IDE rename and find reference support
        private static readonly Api.Query.NotificationQuery PropertyNames = null;


        public static ANotificationQuery With(){
                return new ANotificationQuery();
        }

        public static IMatcher<Api.Query.NotificationQuery> Null(){
                return AnInstance.Null<Api.Query.NotificationQuery>();
        }

        public static IMatcher<Api.Query.NotificationQuery> NotNull(){
                return AnInstance.NotNull<Api.Query.NotificationQuery>();
        }

        public static IMatcher<Api.Query.NotificationQuery> Instance(Api.Query.NotificationQuery expect){
                return AnInstance.SameAs(expect);
        }

        public ANotificationQuery AccountId(Guid? expect) {
            AccountId(AGuid.EqualTo(expect));
            return this;
        }

        public ANotificationQuery AccountIdNull() {
            AccountId(AGuid.Null());
            return this;
        }

        public ANotificationQuery AccountId(IMatcher<Guid?> matcher) {
            WithProperty(()=>PropertyNames.AccountId,matcher);
            return this;
        }

        public ANotificationQuery Id(Guid? expect) {
            Id(AGuid.EqualTo(expect));
            return this;
        }

        public ANotificationQuery IdNull() {
            Id(AGuid.Null());
            return this;
        }

        public ANotificationQuery Id(IMatcher<Guid?> matcher) {
            WithProperty(()=>PropertyNames.Id,matcher);
            return this;
        }
    }
}

namespace TestFirst.Net.Examples {

    public class AResponse : PropertyMatcher<Api.Query.NotificationQuery.Response>{

        // provide IDE rename and find reference support
        private static readonly Api.Query.NotificationQuery.Response PropertyNames = null;


        public static AResponse With(){
                return new AResponse();
        }

        public static IMatcher<Api.Query.NotificationQuery.Response> Null(){
                return AnInstance.Null<Api.Query.NotificationQuery.Response>();
        }

        public static IMatcher<Api.Query.NotificationQuery.Response> NotNull(){
                return AnInstance.NotNull<Api.Query.NotificationQuery.Response>();
        }

        public static IMatcher<Api.Query.NotificationQuery.Response> Instance(Api.Query.NotificationQuery.Response expect){
                return AnInstance.SameAs(expect);
        }

        public AResponse ResultNull() {
            Result(AnInstance.EqualTo<Api.Notification>(null));
            return this;
        }

        public AResponse Result(IMatcher<Api.Notification> matcher) {
            WithProperty(()=>PropertyNames.Result,matcher);
            return this;
        }

        public AResponse ResultsNull() {
            Results(AnInstance.EqualTo<IEnumerable<Api.Notification>>(null));
            return this;
        }

        public AResponse Results(IMatcher<IEnumerable<Api.Notification>> matcher) {
            WithProperty(()=>PropertyNames.Results,matcher);
            return this;
        }
    }
}
