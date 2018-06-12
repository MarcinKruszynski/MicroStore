using BookingService.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingService.Data
{
    public class BookingContextSeed
    {
        public static async Task SeedAsync(BookingContext context)
        {
            context.Database.Migrate();

            if (!context.BookingStatus.Any())
            {
                context.BookingStatus.AddRange(GetPredefinedBookingStatus());

                await context.SaveChangesAsync();
            }            
        }

        private static IEnumerable<BookingStatus> GetPredefinedBookingStatus()
        {
            return new List<BookingStatus>()
            {
                BookingStatus.Submitted,
                BookingStatus.CheckingAvailability,
                BookingStatus.StockConfirmed,
                BookingStatus.Paid,
                BookingStatus.Cancelled
            };
        }
    }
}
