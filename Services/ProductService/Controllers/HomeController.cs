using Microsoft.AspNetCore.Mvc;

namespace ProductService.Controllers
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
