using Microsoft.AspNetCore.Mvc;

namespace ZustSN.WebUI.Controllers
{
    public class FeedController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
