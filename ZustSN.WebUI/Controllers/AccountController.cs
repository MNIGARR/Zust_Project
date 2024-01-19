using Microsoft.AspNetCore.Mvc;

namespace ZustSN.WebUI.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
