using BookingService.Interfaces;
using /*MicroStore.Services.IntegrationEvents.*/Events;
using NServiceBus;
using System.Threading.Tasks;

namespace BookingService.IntegrationEvents.EventHandling
{
    public class BookingPaymentFailedIntegrationEventHandler :
        IHandleMessages<BookingPaymentFailedIntegrationEvent>
    {
        private readonly IBookingRepository _bookingRepository;

        public BookingPaymentFailedIntegrationEventHandler(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task Handle(BookingPaymentFailedIntegrationEvent message, IMessageHandlerContext context)
        {
            var booking = await _bookingRepository.GetAsync(message.BookingId);

            booking.SetCancelledStatus();

            await _bookingRepository.SaveChangesAsync();
        }
    }
}
