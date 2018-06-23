using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NotificationService.Models;
using System.Net;
using WebPush;

namespace NotificationService.Controllers
{    
    [Route("api/v1/[controller]")]
    [Authorize]
    public class NotificationsController : Controller
    {
        //test
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        public IActionResult Post([FromBody]Models.PushSubscription value)
        {
            var vapidDetails = new VapidDetails(
                    @"mailto:pop365@outlook.com",
                    "BEoYccO5q2KhdfX1Wt0U4tpWzZBRa4FGImrvzXqu073QmO2V7r3aGv0MOf-BizbWX5V3H65ns3p7uZ07DMMrjvk",
                    "wD97u6MzPoW77gtRmOiu0gpNON7wE9zEJIOXyGizO2c"
                );            

            var payload = JsonConvert.SerializeObject(
                new
                {
                    bookingId = 1,
                    productName = "Progressive apps - 31.06.2018",
                    quantity = 1
                }
            );

            var webPushClient = new WebPushClient();

            var subscription = new WebPush.PushSubscription(value.Endpoint, value.Keys["p256dh"], value.Keys["auth"]);
            
            try
            {
                webPushClient.SendNotification(subscription, payload, vapidDetails);                    
            }
            catch (WebPushException ex)
            {
                //to do
            }            

            return Ok();
        }
    }
}
