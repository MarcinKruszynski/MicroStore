using MicroStore.Services.IntegrationEvents.Events;
using NServiceBus;
using ProductService.Data;
using System.Threading.Tasks;

namespace ProductService.IntegrationEvents.EventHandling
{
    public class BookingStatusChangedToPaidIntegrationEventHandler:
        IHandleMessages<BookingStatusChangedToPaidIntegrationEvent>
    {
        private readonly ProductContext _productContext;

        public BookingStatusChangedToPaidIntegrationEventHandler(ProductContext productContext)
        {
            _productContext = productContext;
        }

        public async Task Handle(BookingStatusChangedToPaidIntegrationEvent message, IMessageHandlerContext context)
        {
            var bookingStockItem = message.BookingStockItem;
            var productItem = _productContext.ProductItems.Find(bookingStockItem.ProductId);

            productItem.RemoveStock(bookingStockItem.Quantity);            

            await _productContext.SaveChangesAsync();
        }
    }    
}
