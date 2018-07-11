using BookingAggregator.Model;
using System.Threading.Tasks;

namespace BookingAggregator.Interfaces
{
    public interface IProductService
    {
        Task<ProductItem> GetProductItem(int id);
    }
}
