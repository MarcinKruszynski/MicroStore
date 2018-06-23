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
            var productItem = _productContext.ProductItems.Find(message.ProductId);

            productItem.RemoveStock(message.Quantity);            

            await _productContext.SaveChangesAsync();
        }
    }    
}
