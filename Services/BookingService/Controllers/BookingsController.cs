using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using BookingService.Interfaces;
using BookingService.Model;
using BookingService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MicroStore.Services.IntegrationEvents.Events;
using NServiceBus;

namespace BookingService.Controllers
{
    [Route("api/v1/[controller]")]
    [Authorize]
    public class BookingsController : Controller
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IIdentityService _identityService;
        private readonly IEndpointInstance _endpoint;

        public BookingsController(IBookingRepository bookingRepository, IIdentityService identityService, IEndpointInstance endpoint)
        {
            _bookingRepository = bookingRepository;
            _identityService = identityService;
            _endpoint = endpoint;
        }

        [Route("checkout")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Checkout([FromBody]BookingCheckout bookingCheckout)
        {
            var userId = _identityService.GetUserIdentity();
            
            //to do: check unique request id

            var booking = new Booking(bookingCheckout.ProductId, bookingCheckout.ProductName, bookingCheckout.UnitPrice, bookingCheckout.Quantity, userId);

            _bookingRepository.Add(booking);

            await _bookingRepository.SaveChangesAsync();

            var eventMessage = new BookingStartedIntegrationEvent(userId, booking.Id, bookingCheckout.ProductId, bookingCheckout.Quantity);
            await _endpoint.Publish(eventMessage);

            return Accepted();
        }
    }
}
