namespace /*MicroStore.Services.IntegrationEvents.*/Events
{
    public class BookingStatusStockConfirmedIntegrationEvent
    {
        public int BookingId { get; }

        public BookingStatusStockConfirmedIntegrationEvent(int bookingId) => BookingId = bookingId;
    }
}
