using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BookingService.Data;
using BookingService.Interfaces;
using BookingService.Model;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly BookingContext _context;

        public BookingRepository(BookingContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Booking Add(Booking booking)
        {
            return _context.Bookings.Add(booking).Entity;
        }

        public void Update(Booking booking)
        {
            _context.Entry(booking).State = EntityState.Modified;
        }

        public async Task<Booking> GetAsync(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {                
                await _context.Entry(booking)
                    .Reference(i => i.Status).LoadAsync();                
            }

            return booking;
        }        

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }        
    }
}
