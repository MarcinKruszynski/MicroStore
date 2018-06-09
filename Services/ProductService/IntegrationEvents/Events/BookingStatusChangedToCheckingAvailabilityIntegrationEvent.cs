namespace MicroStore.Services.IntegrationEvents.Events
{
    public class BookingStatusChangedToCheckingAvailabilityIntegrationEvent
    {
        public int BookingId { get; set; }
        public BookingStockItem BookingStockItem { get; set; }        

        public BookingStatusChangedToCheckingAvailabilityIntegrationEvent(
            int bookingId, BookingStockItem bookingStockItem)
        {
            BookingId = bookingId;
            BookingStockItem = bookingStockItem;
        }
    }

    public class BookingStockItem
    {
        public int ProductId { get; }
        public int Quantity { get; }

        public BookingStockItem(int productId, int quantity)
        {
            ProductId = productId;
            Quantity = quantity;
        }
    }
}
