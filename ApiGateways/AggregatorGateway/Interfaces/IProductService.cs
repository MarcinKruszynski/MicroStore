using AggregatorGateway.Model;
using System.Threading.Tasks;

namespace AggregatorGateway.Interfaces
{
    public interface IProductService
    {
        Task<ProductItem> GetProductItem(int id);
    }
}
