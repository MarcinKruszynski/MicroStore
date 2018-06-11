using BookingService.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookingService.Model
{
    public class BookingStatus: Enumeration
    {
        public static BookingStatus Submitted = new BookingStatus(1, nameof(Submitted).ToLowerInvariant());
        public static BookingStatus CheckingAvailability = new BookingStatus(2, nameof(CheckingAvailability).ToLowerInvariant());
        public static BookingStatus StockConfirmed = new BookingStatus(3, nameof(StockConfirmed).ToLowerInvariant());
        public static BookingStatus Paid = new BookingStatus(4, nameof(Paid).ToLowerInvariant());        
        public static BookingStatus Cancelled = new BookingStatus(6, nameof(Cancelled).ToLowerInvariant());

        protected BookingStatus()
        {
        }

        public BookingStatus(int id, string name)
            : base(id, name)
        {
        }

        public static IEnumerable<BookingStatus> List() =>
            new[] { Submitted, CheckingAvailability, StockConfirmed, Paid, Cancelled };

        public static BookingStatus FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new ArgumentException($"Possible values for BookingStatus: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static BookingStatus From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new ArgumentException($"Possible values for BookingStatus: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}
