using System.Threading.Tasks;

namespace AggregatorGateway.Interfaces
{
    public interface IBookingApiClient
    {
        Task CreateBooking(int productId, string productName, decimal unitPrice, int quantity);
    }
}
