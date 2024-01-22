using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ZustSN.Entities;
using ZustSN.WebUI.Helpers;
using ZustSN.WebUI.Models;

namespace ZustSN.WebUI.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<ZustIdentityUser> _userManager;
        private RoleManager<ZustIdentityRole> _roleManager;
        private SignInManager<ZustIdentityUser> _signInManager;
        private IWebHostEnvironment _webHost;
        private ZustIdentityDBContext _dbContext;
        private readonly IPasswordHasher<ZustIdentityUser> _passHasher;

        public AccountController(
           UserManager<ZustIdentityUser> userManager,
           RoleManager<ZustIdentityRole> roleManager,
           IPasswordHasher<ZustIdentityUser> passwordHasher,
           ZustIdentityDBContext context,
           IWebHostEnvironment webHost,
           SignInManager<ZustIdentityUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _passHasher = passwordHasher;
            _dbContext = context;
            _webHost = webHost;
        }

        public IActionResult Login()
        {
            return View("Login");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(value => value.Errors);
            }
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.EmailOrUsername, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    var user = _dbContext.Users.SingleOrDefault(user => user.UserName == model.EmailOrUsername);
                    if (user != null)
                    {
                        user.ConnectTime = DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString();
                        user.IsOnline = true;
                        _dbContext.Users.Update(user);
                        await _dbContext.SaveChangesAsync();
                    }
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Incorrect Username or Password.");
            }
            return View(model);
        }
        public IActionResult ForgotPassword()
        {
            return View("ForgotPassword");
        }


        public IActionResult Register()
        {
            return View("Register");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var helper = new ImageHelper(_webHost);
                if (model.File != null)
                {
                    model.ImageUrl = await helper.SaveFile(model.File);
                }

                ZustIdentityUser user = new ZustIdentityUser
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = model.Username,
                    Email = model.Email,
                    ImageUrl = model.ImageUrl,
                };

                IdentityResult result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    if (!await _roleManager.RoleExistsAsync("Admin"))
                    {
                        ZustIdentityRole role = new ZustIdentityRole{ Name = "Admin" };
                        IdentityResult roleResult = await _roleManager.CreateAsync(role);
                        if (!roleResult.Succeeded)
                        {
                            ModelState.AddModelError("", "Can not added as Admin!");
                            return View(model);
                        }
                    }
                    _userManager.AddToRoleAsync(user, "Admin").Wait();
                    return RedirectToAction("Login", "Account");
                }
            }
            return View(model);
        }
        public async Task<IActionResult> LogOut()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            user.IsOnline = false;
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("Login", "Account");
        }      
    }
}