using Microsoft.Extensions.Logging;
using MicroStore.Services.IntegrationEvents.Events;
using NServiceBus;
using System.Threading.Tasks;

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
            //to do: send notification

            _logger.LogInformation($"Order {message.BookingId} payment notification was sent.");

            return Task.CompletedTask;
        }
    }    
}
