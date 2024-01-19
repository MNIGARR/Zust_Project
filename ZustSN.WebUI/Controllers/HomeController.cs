using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ZustSN.WebUI.Models;

namespace ZustSN.WebUI.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

       
    }
}