using MicroStore.Services.IntegrationEvents.Events;
using NServiceBus;
using NServiceBus.Persistence.Sql;
using System;
using System.Threading.Tasks;

namespace BookingService.Sagas
{
    public class BookingSaga : SqlSaga<BookingSagaData>,
        IAmStartedByMessages<BookingStartedIntegrationEvent>
    {
        protected override void ConfigureMapping(IMessagePropertyMapper mapper)
        {
            mapper.ConfigureMapping<BookingStartedIntegrationEvent>(_ => _.BookingId);
        }

        protected override string CorrelationPropertyName => nameof(BookingSagaData.BookingId);

        public async Task Handle(BookingStartedIntegrationEvent message, IMessageHandlerContext context)
        {
            Data.UserId = message.UserId;

            var bookingStockItem = new BookingStockItem(message.ProductId, message.Units);
            var @event = new BookingStatusChangedToCheckingAvailabilityIntegrationEvent(message.BookingId, bookingStockItem);

            await context.Publish(@event);            
            
        }
    }

    public class BookingSagaData : IContainSagaData
    {
        public Guid Id { get; set; }
        public string Originator { get; set; }
        public string OriginalMessageId { get; set; }

        public int BookingId { get; set; }
        public string UserId { get; set; }
        public bool StockConfirmed { get; set; }
    }
}
