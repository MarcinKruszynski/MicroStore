using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using NotificationService.Data;
using NotificationService.Interfaces;
using NotificationService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NotificationService.Repositories
{
    public class PushSubscriptionRepository: IPushSubscriptionRepository
    {
        private readonly PushSubscriptionContext _context;

        public PushSubscriptionRepository(IOptions<MongoSettings> settings)
        {
            _context = new PushSubscriptionContext(settings);
        }

        public async Task AddSubscriptionAsync(PushSubscription subscription)
        {
            await _context.PushSubscriptions.InsertOneAsync(subscription);
        }

        public async Task<List<PushSubscription>> GetSubscriptionListAsync()
        {
            return await _context.PushSubscriptions.Find(new BsonDocument()).ToListAsync();
        }
    }
}
