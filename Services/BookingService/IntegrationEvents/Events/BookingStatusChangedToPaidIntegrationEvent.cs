namespace MicroStore.Services.IntegrationEvents.Events
{
    public class BookingStatusChangedToPaidIntegrationEvent
    {
        public int BookingId { get; set; }
        public BookingStockItem BookingStockItem { get; set; }

        public BookingStatusChangedToPaidIntegrationEvent(
            int bookingId, BookingStockItem bookingStockItem)
        {
            BookingId = bookingId;
            BookingStockItem = bookingStockItem;
        }
    }
}
