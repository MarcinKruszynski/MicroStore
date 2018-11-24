using Microsoft.Extensions.Logging;
using /*MicroStore.Services.IntegrationEvents.*/Events;
using Newtonsoft.Json;
using NotificationService.Interfaces;
using NServiceBus;
using System.Linq;
using System.Threading.Tasks;
using WebPush;

namespace NotificationService.IntegrationEvents.EventHandling
{
    public class BookingStatusChangedToPaidIntegrationEventHandler:
        IHandleMessages<BookingStatusChangedToPaidIntegrationEvent>
    {
        private readonly ILogger _logger;
        private readonly IPushSubscriptionRepository _subscriptionRepository;

        public BookingStatusChangedToPaidIntegrationEventHandler(ILogger<BookingStatusChangedToPaidIntegrationEventHandler> logger, IPushSubscriptionRepository subscriptionRepository)
        {
            _logger = logger;
            _subscriptionRepository = subscriptionRepository;
        }

        public async Task Handle(BookingStatusChangedToPaidIntegrationEvent message, IMessageHandlerContext context)
        {
            var vapidDetails = new VapidDetails(
                    @"mailto:pop365@outlook.com",
                    "BEoYccO5q2KhdfX1Wt0U4tpWzZBRa4FGImrvzXqu073QmO2V7r3aGv0MOf-BizbWX5V3H65ns3p7uZ07DMMrjvk",
                    "wD97u6MzPoW77gtRmOiu0gpNON7wE9zEJIOXyGizO2c"
                );

            var subscriptions = await _subscriptionRepository.GetSubscriptionListAsync(message.UserId);

            var payload = JsonConvert.SerializeObject(
                new
                {
                    notification = new
                    {
                        title = "Micro Store",
                        body = "Booking " + message.BookingId + " has been paid."
                    }                    
                }
            );

            var webPushClient = new WebPushClient();

            foreach(var subscription in subscriptions.Select(s => new WebPush.PushSubscription(s.Endpoint, s.P256DH, s.Auth)))
            {
                try
                {
                    webPushClient.SendNotification(subscription, payload, vapidDetails);

                    _logger.LogInformation($"Order {message.BookingId} payment notification was sent.");
                }
                catch(WebPushException ex)
                {
                    //to do
                }                
            }            
        }
    }    
}
