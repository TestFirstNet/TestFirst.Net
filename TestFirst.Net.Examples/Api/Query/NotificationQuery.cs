using System;
using TestFirst.Net.Examples.Net;

namespace TestFirst.Net.Examples.Api.Query
{
    public class NotificationQuery : IReturn<NotificationQuery.Response>
    {
        public Guid? Id { get; set; }
        public Guid? AccountId { get; set; }

        public class Response : QueryResponse<Notification>
        {
        }
    }
}
