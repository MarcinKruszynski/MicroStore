using System.Threading.Tasks;
using MicroStore.Services.IntegrationEvents.Events;
using NServiceBus;
using ProductService.Data;

namespace ProductService.IntegrationEvents.EventHandling
{
    public class BookingStatusChangedToCheckingAvailabilityIntegrationEventHandler :
        IHandleMessages<BookingStatusChangedToCheckingAvailabilityIntegrationEvent>
    {
        private readonly ProductContext _productContext;

        public BookingStatusChangedToCheckingAvailabilityIntegrationEventHandler(ProductContext productContext)
        {
            _productContext = productContext;
        }

        public async Task Handle(BookingStatusChangedToCheckingAvailabilityIntegrationEvent message, IMessageHandlerContext context)
        {
            var bookingStockItem = message.BookingStockItem;
            var productItem = _productContext.ProductItems.Find(bookingStockItem.ProductId);

            if(productItem.StockQuantity >= bookingStockItem.Quantity)
            {
                await context.Publish(new BookingStockConfirmedIntegrationEvent(message.BookingId));
            }
            else
            {
                await context.Publish(new BookingStockRejectedIntegrationEvent(message.BookingId));
            }
        }
    }
}
