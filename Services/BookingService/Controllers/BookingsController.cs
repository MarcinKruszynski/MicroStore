using System.Net;
using System.Threading.Tasks;
using BookingService.Model;
using BookingService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingService.Controllers
{
    [Route("api/v1/[controller]")]
    [Authorize]
    public class BookingsController : Controller
    {
        private readonly IIdentityService _identityService;

        public BookingsController(IIdentityService identityService)
        {
            _identityService = identityService;            
        }


        [Route("checkout")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Checkout([FromBody]BookingCheckout bookingCheckout, [FromHeader(Name = "x-requestid")] string requestId)
        {
            var userId = _identityService.GetUserIdentity();

            //to do            

            return Accepted();
        }
    }
}
