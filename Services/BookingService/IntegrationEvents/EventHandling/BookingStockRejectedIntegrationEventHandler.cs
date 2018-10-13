using BookingService.Interfaces;
using /*MicroStore.Services.IntegrationEvents.*/Events;
using NServiceBus;
using System.Threading.Tasks;

namespace BookingService.IntegrationEvents.EventHandling
{
    public class BookingStockRejectedIntegrationEventHandler: IHandleMessages<BookingStockRejectedIntegrationEvent>
    {
        private readonly IBookingRepository _bookingRepository;

        public BookingStockRejectedIntegrationEventHandler(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task Handle(BookingStockRejectedIntegrationEvent message, IMessageHandlerContext context)
        {
            var booking = await _bookingRepository.GetAsync(message.BookingId);            

            booking.SetCancelledStatus();

            await _bookingRepository.SaveChangesAsync();
        }
    }
}
