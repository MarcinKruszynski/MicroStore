namespace MicroStore.Services.IntegrationEvents.Events
{
    public class BookingPaymentFailedIntegrationEvent
    {
        public int BookingId { get; }

        public BookingPaymentFailedIntegrationEvent(int bookingId) => BookingId = bookingId;
    }
}
