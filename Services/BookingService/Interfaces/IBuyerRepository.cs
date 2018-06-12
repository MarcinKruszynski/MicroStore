using BookingService.Model;
using System.Threading.Tasks;

namespace BookingService.Interfaces
{
    interface IBuyerRepository
    {
        Buyer Add(Buyer buyer);

        Buyer Update(Buyer buyer);

        Task<Buyer> FindAsync(string identityGuid);

        Task SaveChangesAsync();
    }
}
