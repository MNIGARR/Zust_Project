using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ZustSN.Entities;
namespace ZustSN.WebUI.Controllers
{
    public class FeedController : Controller
    {

        private UserManager<ZustIdentityUser> _userManager;
        private ZustIdentityDBContext _dbContext;
        public FeedController(UserManager<ZustIdentityUser> userManager, ZustIdentityDBContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Favorites()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            ViewBag.User = new
            {
                Username = user.UserName,
                Email = user.Email,
                ImageUrl = user.ImageUrl
                
            };
            return View("Favorites");
        }
        
    }
}
