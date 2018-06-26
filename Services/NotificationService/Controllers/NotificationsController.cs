using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotificationService.Interfaces;
using NotificationService.Services;
using System.Net;
using System.Threading.Tasks;

namespace NotificationService.Controllers
{
    [Route("api/v1/[controller]")]
    [Authorize]
    public class NotificationsController: Controller
    {
        private readonly IPushSubscriptionRepository _subscriptionRepository;
        private readonly IIdentityService _identityService;

        public NotificationsController(IPushSubscriptionRepository subscriptionRepository, IIdentityService identityService)
        {
            _subscriptionRepository = subscriptionRepository;
            _identityService = identityService;
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Post([FromBody]Dto.PushSubscriptionDTO value)
        {
            var userId = _identityService.GetUserIdentity();

            await _subscriptionRepository.AddSubscriptionAsync(new Models.PushSubscription
            {
                UserId = userId,
                Endpoint = value.Endpoint,
                P256DH = value.Keys["p256dh"],
                Auth = value.Keys["auth"]
            });

            return Ok();
        }
    }
}
