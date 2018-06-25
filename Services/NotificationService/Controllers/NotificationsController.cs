using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotificationService.Interfaces;
using System.Net;
using System.Threading.Tasks;

namespace NotificationService.Controllers
{
    [Route("api/v1/[controller]")]
    [Authorize]
    public class NotificationsController: Controller
    {
        private readonly IPushSubscriptionRepository _subscriptionRepository;

        public NotificationsController(IPushSubscriptionRepository subscriptionRepository)
        {
            _subscriptionRepository = subscriptionRepository;            
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Post([FromBody]Dto.PushSubscriptionDTO value)
        {
            await _subscriptionRepository.AddSubscriptionAsync(new Models.PushSubscription
            {
                Endpoint = value.Endpoint,
                P256DH = value.Keys["p256dh"],
                Auth = value.Keys["auth"]
            });

            return Ok();
        }
    }
}
