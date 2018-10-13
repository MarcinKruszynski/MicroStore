using System.Threading.Tasks;
using /*MicroStore.Services.IntegrationEvents.*/Events;
using NServiceBus;
using ProductService.Data;

namespace ProductService.IntegrationEvents.EventHandling
{
    public class BookingStatusChangedToCheckIntegrationEventHandler :
        IHandleMessages<BookingStatusChangedToCheckIntegrationEvent>
    {
        private readonly ProductContext _productContext;

        public BookingStatusChangedToCheckIntegrationEventHandler(ProductContext productContext)
        {
            _productContext = productContext;
        }

        public async Task Handle(BookingStatusChangedToCheckIntegrationEvent message, IMessageHandlerContext context)
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
