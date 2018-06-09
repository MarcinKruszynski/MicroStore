namespace MicroStore.Services.IntegrationEvents.Events
{
    public class BookingStockRejectedIntegrationEvent
    {
        public int BookingId { get; }

        public BookingStockRejectedIntegrationEvent(int bookingId) => BookingId = bookingId;
    }
}
