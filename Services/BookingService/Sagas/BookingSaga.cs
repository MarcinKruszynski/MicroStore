using MicroStore.Services.IntegrationEvents.Events;
using NServiceBus;
using NServiceBus.Persistence.Sql;
using System;
using System.Threading.Tasks;

namespace BookingService.Sagas
{
    public class BookingSaga : SqlSaga<BookingSagaData>,
        IAmStartedByMessages<BookingStartedIntegrationEvent>,
        IHandleMessages<BookingStockConfirmedIntegrationEvent>,
        IHandleMessages<BookingStockRejectedIntegrationEvent>,
        IHandleMessages<BookingPaymentSuccededIntegrationEvent>,
        IHandleMessages<BookingPaymentFailedIntegrationEvent>
    {
        protected override void ConfigureMapping(IMessagePropertyMapper mapper)
        {
            mapper.ConfigureMapping<BookingStartedIntegrationEvent>(_ => _.BookingId);
            mapper.ConfigureMapping<BookingStockConfirmedIntegrationEvent>(_ => _.BookingId);
            mapper.ConfigureMapping<BookingStockRejectedIntegrationEvent>(_ => _.BookingId);
            mapper.ConfigureMapping<BookingPaymentSuccededIntegrationEvent>(_ => _.BookingId);
            mapper.ConfigureMapping<BookingPaymentFailedIntegrationEvent>(_ => _.BookingId);
        }

        protected override string CorrelationPropertyName => nameof(BookingSagaData.BookingId);

        public async Task Handle(BookingStartedIntegrationEvent message, IMessageHandlerContext context)
        {
            Data.UserId = message.UserId;

            var bookingStockItem = new BookingStockItem(message.ProductId, message.Units);
            var ev = new BookingStatusChangedToCheckingAvailabilityIntegrationEvent(message.BookingId, bookingStockItem);

            await context.Publish(ev);             
        }

        public async Task Handle(BookingStockConfirmedIntegrationEvent message, IMessageHandlerContext context)
        {
            Data.StockConfirmed = true;

            var ev = new BookingStatusChangedToStockConfirmedIntegrationEvent(message.BookingId);

            await context.Publish(ev);
        }

        public Task Handle(BookingStockRejectedIntegrationEvent message, IMessageHandlerContext context)
        {
            MarkAsComplete();

            return Task.CompletedTask;
        }

        public Task Handle(BookingPaymentSuccededIntegrationEvent message, IMessageHandlerContext context)
        {
            MarkAsComplete();

            return Task.CompletedTask;
        }

        public Task Handle(BookingPaymentFailedIntegrationEvent message, IMessageHandlerContext context)
        {
            MarkAsComplete();

            return Task.CompletedTask;
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
