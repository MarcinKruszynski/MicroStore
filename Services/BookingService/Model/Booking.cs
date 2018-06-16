using System;

namespace BookingService.Model
{
    public class Booking
    {
        public int Id { get; private set; }

        private DateTime _date;

        private string _userId;

        public BookingStatus Status { get; private set; }
        private int _statusId;

        public int ProductId { get; private set; }

        private string _productName;
        private decimal _unitPrice;
        private int _units;

        public Booking(int productId, string productName, decimal unitPrice, int units = 1, string userId = null)
        {
            _userId = userId;
            _statusId = BookingStatus.Submitted.Id;
            _date = DateTime.UtcNow;

            ProductId = productId;

            _productName = productName;
            _unitPrice = unitPrice;            
            _units = units;
        }        

        public int GetUnits() => _units;        

        public decimal GetUnitPrice() => _unitPrice;        

        public string GetOrderItemProductName() => _productName;


        public void SetCheckingAvailability()
        {
            _statusId = BookingStatus.CheckingAvailability.Id;
        }

        public void SetStockConfirmedStatus()
        {
            _statusId = BookingStatus.StockConfirmed.Id;            
        }

        public void SetPaidStatus()
        {
            if (_statusId != BookingStatus.StockConfirmed.Id)
            {
                throw new Exception($"Not possible to change booking status from {Status.Name} to {BookingStatus.Paid.Name}.");
            }

            _statusId = BookingStatus.Paid.Id;            
        }

        public void SetCancelledStatus()
        {
            _statusId = BookingStatus.Cancelled.Id;            
        }
    }
}
