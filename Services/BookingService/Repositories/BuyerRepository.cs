using BookingService.Data;
using BookingService.Interfaces;
using BookingService.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BookingService.Repositories
{
    public class BuyerRepository : IBuyerRepository
    {
        private readonly BookingContext _context;

        public BuyerRepository(BookingContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Buyer Add(Buyer buyer)
        {
            if (buyer.Id == 0)
            {
                return _context.Buyers
                    .Add(buyer)
                    .Entity;
            }
            else
            {
                return buyer;
            }
        }

        public Buyer Update(Buyer buyer)
        {
            return _context.Buyers
                    .Update(buyer)
                    .Entity;
        }

        public async Task<Buyer> FindAsync(string identityGuid)
        {
            var buyer = await _context.Buyers                
                .Where(b => b.IdentityGuid == identityGuid)
                .SingleOrDefaultAsync();

            return buyer;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
