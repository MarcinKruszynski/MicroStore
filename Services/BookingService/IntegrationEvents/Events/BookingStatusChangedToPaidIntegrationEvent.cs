namespace MicroStore.Services.IntegrationEvents.Events
{
    public class BookingStatusChangedToPaidIntegrationEvent
    {
        public int BookingId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }

        public BookingStatusChangedToPaidIntegrationEvent(
            int bookingId, int productId, string productName, int quantity)
        {
            BookingId = bookingId;
            ProductId = productId;
            ProductName = productName;
            Quantity = quantity;
        }
    }
}
