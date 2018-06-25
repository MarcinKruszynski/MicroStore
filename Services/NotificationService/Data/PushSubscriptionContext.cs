using Microsoft.Extensions.Options;
using MongoDB.Driver;
using NotificationService.Models;

namespace NotificationService.Data
{
    public class PushSubscriptionContext
    {
        private readonly IMongoDatabase _database = null;

        public PushSubscriptionContext(IOptions<MongoSettings> settings)
        {
            var client = new MongoClient(settings.Value.MongoConnectionString);
            if (client != null)
                _database = client.GetDatabase(settings.Value.MongoDatabase);
        }

        public IMongoCollection<PushSubscription> PushSubscriptions
        {
            get
            {
                return _database.GetCollection<PushSubscription>("PushSubscriptions");
            }
        }
    }
}
