using System.Threading.Tasks;

namespace BookingAggregator.Interfaces
{
    public interface IBookingApiClient
    {
        Task CreateBooking(int productId, string productName, decimal unitPrice, int quantity);
    }
}
