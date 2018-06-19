namespace MicroStore.Services.IntegrationEvents.Events
{
    public class BookingStatusChangedToStockConfirmedIntegrationEvent
    {
        public int BookingId { get; }

        public BookingStatusChangedToStockConfirmedIntegrationEvent(int bookingId) => BookingId = bookingId;
    }
}
