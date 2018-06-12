using BookingService.Model;
using System.Threading;
using System.Threading.Tasks;

namespace BookingService.Interfaces
{
    public interface IBookingRepository
    {
        Booking Add(Booking booking);

        void Update(Booking booking);

        Task<Booking> GetAsync(int id);

        Task SaveChangesAsync();
    }
}
