using Microsoft.Extensions.Logging;
using /*MicroStore.Services.IntegrationEvents.*/Events;
using NServiceBus;
using ProductService.Data;
using Serilog;
using System.Threading.Tasks;

namespace ProductService.IntegrationEvents.EventHandling
{
    public class BookingStatusChangedToPaidIntegrationEventHandler:
        IHandleMessages<BookingStatusChangedToPaidIntegrationEvent>
    {
        private readonly ILogger<BookingStatusChangedToPaidIntegrationEventHandler> _logger;
        private readonly ProductContext _productContext;

        public BookingStatusChangedToPaidIntegrationEventHandler(ILogger<BookingStatusChangedToPaidIntegrationEventHandler> logger, ProductContext productContext)
        {
            _logger = logger;
            _productContext = productContext;
        }

        public async Task Handle(BookingStatusChangedToPaidIntegrationEvent message, IMessageHandlerContext context)
        {
            _logger.LogTrace("Handle message {@message}", message);

            var productItem = _productContext.ProductItems.Find(message.ProductId);

            productItem.RemoveStock(message.Quantity);            

            await _productContext.SaveChangesAsync();

            _logger.LogInformation("Product {@product} after removing stock", productItem);
        }
    }    
}
