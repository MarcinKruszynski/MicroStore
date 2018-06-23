using Microsoft.Extensions.Logging;
using MicroStore.Services.IntegrationEvents.Events;
using Newtonsoft.Json;
using NotificationService.Models;
using NServiceBus;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebPush;

namespace NotificationService.IntegrationEvents.EventHandling
{
    public class BookingStatusChangedToPaidIntegrationEventHandler:
        IHandleMessages<BookingStatusChangedToPaidIntegrationEvent>
    {
        private readonly ILogger _logger;

        public BookingStatusChangedToPaidIntegrationEventHandler(ILogger<BookingStatusChangedToPaidIntegrationEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(BookingStatusChangedToPaidIntegrationEvent message, IMessageHandlerContext context)
        {
            var vapidDetails = new VapidDetails(
                    @"mailto:pop365@outlook.com",
                    "BEoYccO5q2KhdfX1Wt0U4tpWzZBRa4FGImrvzXqu073QmO2V7r3aGv0MOf-BizbWX5V3H65ns3p7uZ07DMMrjvk",
                    "wD97u6MzPoW77gtRmOiu0gpNON7wE9zEJIOXyGizO2c"
                );

            var subscriptions = new List<Models.PushSubscription>(); //await Database.GetPushSubscriptions();

            var payload = JsonConvert.SerializeObject(
                new
                {
                    bookingId = message.BookingId,
                    productId = message.BookingStockItem.ProductId,
                    quantity = message.BookingStockItem.Quantity
                }
            );

            var webPushClient = new WebPushClient();

            foreach(var subscription in subscriptions.Select(s => new WebPush.PushSubscription(s.Endpoint, s.Keys["p256dh"], s.Keys["auth"])))
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

            return Task.CompletedTask;
        }
    }    
}
