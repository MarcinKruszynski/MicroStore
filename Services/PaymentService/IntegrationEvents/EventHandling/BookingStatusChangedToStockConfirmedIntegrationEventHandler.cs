using MicroStore.Services.IntegrationEvents.Events;
using NServiceBus;
using System.Threading.Tasks;

namespace PaymentService.IntegrationEvents.EventHandling
{
    public class BookingStatusChangedToStockConfirmedIntegrationEventHandler:
        IHandleMessages<BookingStatusChangedToStockConfirmedIntegrationEvent>
    {
        public BookingStatusChangedToStockConfirmedIntegrationEventHandler()
        {
        }

        public async Task Handle(BookingStatusChangedToStockConfirmedIntegrationEvent message, IMessageHandlerContext context)
        {
            if (message.BookingId % 2 == 0)
            {
                await context.Publish(new BookingPaymentSuccededIntegrationEvent(message.BookingId));
            }
            else
            {
                await context.Publish(new BookingPaymentFailedIntegrationEvent(message.BookingId));
            }
        }
    }
}
