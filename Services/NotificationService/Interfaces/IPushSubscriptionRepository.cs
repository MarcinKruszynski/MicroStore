using NotificationService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NotificationService.Interfaces
{
    public interface IPushSubscriptionRepository
    {
        Task AddSubscriptionAsync(PushSubscription subscription);

        Task<List<PushSubscription>> GetSubscriptionListAsync();
    }
}
