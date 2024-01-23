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
        private ZustIdentityDBContext _context;
        private readonly IPasswordHasher<ZustIdentityUser> _passwordHasher;

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
            _passwordHasher = passwordHasher;
            _context = context;
            _webHost = webHost;
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
                        ZustIdentityRole role = new ZustIdentityRole
                        {
                            Name = "Admin"
                        };

                        IdentityResult roleResult = await _roleManager.CreateAsync(role);
                        if (!roleResult.Succeeded)
                        {
                            ModelState.AddModelError("", "We can not add the role!");
                            return View(model);
                        }
                    }

                    _userManager.AddToRoleAsync(user, "Admin").Wait();
                    return RedirectToAction("Login", "Account");

                }
            }

            return View(model);
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
                var errors = ModelState.Values.SelectMany(v => v.Errors);
            }
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    var user = _context.Users.SingleOrDefault(u => u.UserName == model.Username);
                    if (user != null)
                    {
                        user.ConnectTime = DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString();
                        user.IsOnline = true;
                        _context.Users.Update(user);
                        await _context.SaveChangesAsync();
                    }
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Invalid Login");
            }
            return View(model);
        }

        public async Task<IActionResult> LogOut()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            currentUser.IsOnline = false;
            _context.Users.Update(currentUser);
            await _context.SaveChangesAsync();
            return RedirectToAction("Login", "Account");
        }

        public IActionResult ForgotPassword()
        {
            return View("ForgotPassword");
        }
    }
}