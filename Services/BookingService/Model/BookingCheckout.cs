using System;

namespace BookingService.Model
{
    public class BookingCheckout
    {
        public string RequestId { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }
    }
}
