using AggregatorGateway.Interfaces;
using AggregatorGateway.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AggregatorGateway.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IBookingApiClient _bookingClient;

        public BookingController(IProductService productService, IBookingApiClient bookingClient)
        {
            _productService = productService;
            _bookingClient = bookingClient;
        }

        [HttpPost]
        [Route("book")]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingRequest data)
        {
            if (data.ProductId <= 0)
            {
                return BadRequest("Need a valid productid");
            }

            var product = await _productService.GetProductItem(data.ProductId);

            if (product == null)
            {
                return BadRequest($"No product found for id {data.ProductId}");
            }

            await _bookingClient.CreateBooking(product.Id, product.Name, product.Price, data.Quantity);

            return Ok();
        }        
    }
}
