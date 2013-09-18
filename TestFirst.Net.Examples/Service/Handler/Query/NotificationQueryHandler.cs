using System.Collections.Generic;
using TestFirst.Net.Examples.Api;
using TestFirst.Net.Examples.Api.Query;

namespace TestFirst.Net.Examples.Service.Handler.Query
{
    internal class NotificationQueryHandler : AbstractQueryHandler<NotificationQuery, NotificationQuery.Response>
    {
        public List<Notification> Notifications { get; set; }

        public override NotificationQuery.Response Handle(NotificationQuery query)
        {
            return new NotificationQuery.Response
                {
                    Results = Notifications == null ? new List<Notification>() : new List<Notification>(Notifications)
                };
        }
    }
}
