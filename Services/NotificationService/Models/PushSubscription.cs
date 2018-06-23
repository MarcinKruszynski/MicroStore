using System.Collections.Generic;

namespace NotificationService.Models
{
    public class PushSubscription
    {
        public string Endpoint { get; set; }

        public IDictionary<string, string> Keys { get; set; }
    }
}
