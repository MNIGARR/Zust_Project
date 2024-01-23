using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ZustSN.WebUI.Models;
using Microsoft.AspNetCore.Identity;
using ZustSN.Entities;

using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;
using Microsoft.EntityFrameworkCore;

namespace ZustSN.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private UserManager<ZustIdentityUser> _userManager;
        private ZustIdentityDBContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        public HomeController(UserManager<ZustIdentityUser> userManager, ZustIdentityDBContext context, Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment)
        {
            _userManager = userManager;
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            ViewBag.User = new
            {
                ImageUrl = user.ImageUrl,
                Username = user.UserName,
                Email = user.Email
            };
            return View();
        }
        public async Task<IActionResult> MyProfile()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            ViewBag.User = new
            {
                ImageUrl = user.ImageUrl,
                Username = user.UserName,
                Email = user.Email
            };
            return View("MyProfile");
        }

        [HttpPost]
        public async Task<IActionResult> Upload(List<IFormFile> files)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var imageUrls = new List<Image>();
            var videoUrls = new List<Video>();

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/assets/posts", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    if (file.ContentType.Contains("image"))
                    {
                        imageUrls = new List<Image>
                        {
                            new Image{Url=$"/assets/posts/{fileName}"}
                        };
                    }
                    else if (file.ContentType.Contains("video"))
                    {
                        imageUrls = new List<Image>
                        {
                            new Image{Url=$"/assets/posts/{fileName}"}
                        };
                    }
                }
            }

            var newPost = new Post
            {
                ImageUrls = imageUrls,
                VideoUrls = videoUrls,
                PostDate = DateTime.UtcNow
                // Populate other fields as needed
            };
            user.Posts.Add(newPost);
            _context.Posts.Add(newPost);
            await _context.SaveChangesAsync();

            return Ok(new { ImageUrls = imageUrls, VideoUrls = videoUrls });
        }


        public async Task<IActionResult> GetPosts()
        {
            var posts = await _context.Posts.ToListAsync();
            return Ok(posts);
        }



        [HttpPost]
        public async Task<IActionResult> CreatePost([FromForm] PostViewModel postModel, List<IFormFile> files, List<IFormFile> videos)
        {
            // Process text content
            var content = postModel.Content;

            // Process files
            foreach (var file in files.Concat(videos))
            {
                if (file.Length > 0)
                {
                    // Define the target path
                    var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "assets", "posts", file.FileName);

                    // Save the file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                }
            }

            return Ok(new { message = "Post created successfully!" });
        }



        public async Task<List<ZustIdentityUser>> GetAllFriends()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var allFriends = _context.Friends
                .Where(f => f.OwnId == user.Id)
                .Select(f => f.YourFriend)
                .ToList();

            return allFriends;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}