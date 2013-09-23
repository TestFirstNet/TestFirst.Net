using System.Collections.Generic;
using NUnit.Framework;
using TestFirst.Net.Examples.Api;
using TestFirst.Net.Examples.Api.Query;
using TestFirst.Net.Examples.Service.Handler.Query;
using TestFirst.Net.Extensions.NUnit;

namespace TestFirst.Net.Examples.Service.Handler
{
    [TestFixture]
    public class NotificationQueryHandlerTest : AbstractNUnitScenarioTest
    {
        [Test]
        public void QueryAll_WithOne_ReturnsOne()
        {
            Notification notification;
            NotificationQueryHandler handler;
            NotificationQuery.Response response;

            Scenario()
                .Given(notification = new Notification())
                .Given(handler = NewHandler().WithNotifications(notification))
                .When(response = handler.Handle(new NotificationQuery()))
                .Then(ExpectThat(response), Is(ANotificationQueryResponse.With().Result(ANotification.Instance(notification))));
        }

        [Test]
        public void QueryAll_WithNone_ReturnsNone()
        {
            Notification notification;
            NotificationQueryHandler handler;
            NotificationQuery.Response response;

            Scenario()
                .Given(handler = NewHandler().WithNoNotifications())
                .When(response = handler.Handle(new NotificationQuery()))
                .Then(ExpectThat(response), Is(ANotificationQueryResponse.With().NoResults()));
        }

        private TheNotificationHandler NewHandler()
        {
            return  new TheNotificationHandler();
        }

        private class TheNotificationHandler : NotificationQueryHandler
        {            
            public TheNotificationHandler WithNotifications(params Notification[] notifications)
            {
                base.Notifications = new List<Notification>(notifications);
                return this;
            }

            public TheNotificationHandler WithNoNotifications()
            {
                base.Notifications = null;
                return this;
            }
        }
    }
}
