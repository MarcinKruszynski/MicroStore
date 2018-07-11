using BookingAggregator.Interfaces;
using BookingAggregator.Model;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace BookingAggregator.Services
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _httpClient;
        private readonly UrlsConfig _urls;

        public ProductService(HttpClient httpClient, IOptions<UrlsConfig> config)
        {
            _httpClient = httpClient;
            _urls = config.Value;
        }

        public async Task<ProductItem> GetProductItem(int id)
        {
            var stringContent = await _httpClient.GetStringAsync(_urls.ProductUrl + "/api/v1/products/" + id);
            var productItem = JsonConvert.DeserializeObject<ProductItem>(stringContent);

            return productItem;
        }
    }
}
