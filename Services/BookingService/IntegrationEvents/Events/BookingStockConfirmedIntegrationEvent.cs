namespace MicroStore.Services.IntegrationEvents.Events
{
    public class BookingStockConfirmedIntegrationEvent
    {
        public int BookingId { get; }

        public BookingStockConfirmedIntegrationEvent(int bookingId) => BookingId = bookingId;
    }
}
