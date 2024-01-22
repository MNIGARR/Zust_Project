using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ZustSN.WebUI.Models;
using Microsoft.AspNetCore.Identity;
using ZustSN.Entities;

namespace ZustSN.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private UserManager<ZustIdentityUser> _userManager;
        private ZustIdentityDBContext _dbContext;

        public HomeController(UserManager<ZustIdentityUser> userManager, ZustIdentityDBContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            ViewBag.User = new
            {
                Username = currentUser.UserName,
                Email = currentUser.Email,
                ImageUrl = currentUser.ImageUrl,
               
            };
            return View();
        }

        public async Task<List<ZustIdentityUser>> GetAllFriends()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var friends = _dbContext.Friends.Where(f => f.OwnId == user.Id)
                .Select(f => f.YourFriend).ToList();
            return friends;
        }

        [HttpPost]
        public IActionResult Post([FromBody] PostViewModel post)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(post);
        }

        public async Task<IActionResult> MyProfile()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            ViewBag.User = new
            {
                Username = currentUser.UserName,
                Email = currentUser.Email,
                ImageUrl = currentUser.ImageUrl,
               
            };
            return View("MyProfile");
        }


    }
}