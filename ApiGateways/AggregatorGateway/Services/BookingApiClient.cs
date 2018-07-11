using BookingAggregator.Interfaces;
using BookingAggregator.Model;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace BookingAggregator.Services
{
    public class BookingApiClient: IBookingApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly UrlsConfig _urls;

        public BookingApiClient(HttpClient httpClient, IOptions<UrlsConfig> config)
        {
            _httpClient = httpClient;
            _urls = config.Value;
        }

        public async Task CreateBooking(int productId, string productName, decimal unitPrice, int quantity)
        {
            var bookingCheckout = new BookingCheckout
            {
                RequestId = Guid.NewGuid().ToString(),
                ProductId = productId,
                ProductName = productName,
                UnitPrice = unitPrice,
                Quantity = quantity
            };

            var content = new StringContent(JsonConvert.SerializeObject(bookingCheckout), System.Text.Encoding.UTF8, "application/json");

            await _httpClient.PostAsync(_urls.BookingUrl + "/api/v1/bookings/add", content);                       
        }
    }
}
