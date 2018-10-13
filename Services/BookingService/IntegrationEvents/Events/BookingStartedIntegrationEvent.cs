namespace /*MicroStore.Services.IntegrationEvents.*/Events
{
    public class BookingStartedIntegrationEvent
    {
        public string UserId { get; set; }

        public int BookingId { get; set; }

        public int ProductId { get; set; }

        public int Units { get; set; }

        public BookingStartedIntegrationEvent(string userId, int bookingId, int productId, int units)
        {
            UserId = userId;
            BookingId = bookingId;
            ProductId = productId;
            Units = units;
        }
    }
}
