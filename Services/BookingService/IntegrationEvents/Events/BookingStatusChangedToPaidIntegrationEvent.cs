namespace MicroStore.Services.IntegrationEvents.Events
{
    public class BookingStatusChangedToPaidIntegrationEvent
    {
        public string UserId { get; set; }
        public int BookingId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }

        public BookingStatusChangedToPaidIntegrationEvent(string userId,
            int bookingId, int productId, string productName, int quantity)
        {
            UserId = userId;
            BookingId = bookingId;
            ProductId = productId;
            ProductName = productName;
            Quantity = quantity;
        }
    }
}
