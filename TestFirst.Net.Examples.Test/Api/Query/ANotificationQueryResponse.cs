namespace TestFirst.Net.Examples.Api.Query
{
    public class ANotificationQueryResponse : AQueryResponse<ANotificationQueryResponse,Notification>
    {
        public new static ANotificationQueryResponse With()
        {
            return new ANotificationQueryResponse();
        }
    }
}
