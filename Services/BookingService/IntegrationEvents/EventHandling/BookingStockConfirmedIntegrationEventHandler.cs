using BookingService.Interfaces;
using MicroStore.Services.IntegrationEvents.Events;
using NServiceBus;
using System.Threading.Tasks;

namespace BookingService.IntegrationEvents.EventHandling
{
    public class BookingStockConfirmedIntegrationEventHandler:
        IHandleMessages<BookingStockConfirmedIntegrationEvent>
    {
        private readonly IBookingRepository _bookingRepository;

        public BookingStockConfirmedIntegrationEventHandler(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task Handle(BookingStockConfirmedIntegrationEvent message, IMessageHandlerContext context)
        {
            var booking = await _bookingRepository.GetAsync(message.BookingId);

            booking.SetStockConfirmedStatus();

            await _bookingRepository.SaveChangesAsync();
        }
    }
}
