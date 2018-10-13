namespace /*MicroStore.Services.IntegrationEvents.*/Events
{
    public class BookingPaymentSuccededIntegrationEvent
    {
        public int BookingId { get; }

        public BookingPaymentSuccededIntegrationEvent(int bookingId) => BookingId = bookingId;
    }
}
