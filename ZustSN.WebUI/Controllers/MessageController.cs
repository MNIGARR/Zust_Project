﻿using Microsoft.AspNetCore.Mvc;

namespace ZustSN.WebUI.Controllers
{
    public class MessageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}