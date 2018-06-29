using Microsoft.AspNetCore.Mvc;

namespace BookingService.Controllers
{
    public class HomeController: Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return new RedirectResult("~/swagger");
        }
    }
}
