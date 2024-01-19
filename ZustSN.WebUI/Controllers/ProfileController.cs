using Microsoft.AspNetCore.Mvc;

namespace ZustSN.WebUI.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
