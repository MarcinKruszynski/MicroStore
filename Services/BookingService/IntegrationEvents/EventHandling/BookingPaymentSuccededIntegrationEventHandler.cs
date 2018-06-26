using BookingService.Interfaces;
using MicroStore.Services.IntegrationEvents.Events;
using NServiceBus;
using System.Threading.Tasks;

namespace BookingService.IntegrationEvents.EventHandling
{
    public class BookingPaymentSuccededIntegrationEventHandler:
        IHandleMessages<BookingPaymentSuccededIntegrationEvent>
    {
        private readonly IBookingRepository _bookingRepository;

        public BookingPaymentSuccededIntegrationEventHandler(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task Handle(BookingPaymentSuccededIntegrationEvent message, IMessageHandlerContext context)
        {
            var booking = await _bookingRepository.GetAsync(message.BookingId);

            booking.SetPaidStatus();

            await _bookingRepository.SaveChangesAsync();

            var ev = new BookingStatusChangedToPaidIntegrationEvent(booking.GetUserId(), message.BookingId, booking.ProductId, booking.GetProductName(), booking.GetUnits());
            await context.Publish(ev);
        }
    }
}
