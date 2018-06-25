using System.Collections.Generic;

namespace NotificationService.Dto
{
    public class PushSubscriptionDTO
    {
        public string Endpoint { get; set; }

        public IDictionary<string, string> Keys { get; set; }
    }
}
