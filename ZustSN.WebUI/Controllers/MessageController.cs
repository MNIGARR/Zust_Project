using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ZustSN.WebUI.Hubs;
using ZustSN.Entities;
using Microsoft.EntityFrameworkCore;
using ZustSN.WebUI.Models;
using Microsoft.AspNetCore.SignalR;

namespace ZustSN.WebUI.Controllers
{
    public class MessageController : Controller
    {
        private UserManager<ZustIdentityUser> _userManager;
        private ZustIdentityDBContext _dbContext;
        private ChatHub _chatHubContext;

        public MessageController(UserManager<ZustIdentityUser> userManager, ZustIdentityDBContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Message()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            ViewBag.User = new
            {
                Email = user.Email,
                Username = user.UserName,
                ImageUrl = user.ImageUrl,

            };
            return View("Message");
        }

        public async Task<IActionResult> GetChatHistory(string senderId, string receiverId)
        {
            try
            {
                var messages = await _dbContext.Messages.Where(m => (m.SenderId == senderId && m.ReceiverId == receiverId)
                || (m.SenderId == receiverId && m.ReceiverId == senderId))
                    .OrderBy(m => m.DateTime)
                    .ToListAsync();

                return Json(messages);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public async Task<IActionResult> LiveChat()
        {
            var current = await _userManager.GetUserAsync(HttpContext.User);
            var users = await _dbContext.Users
                .Where(u => u.Id != current.Id && u.IsOnline)
                .ToListAsync();
            ViewBag.User = new
            {
                Id = current.Id,
                Email = current.Email,
                Username = current.UserName,
                ImageUrl = current.ImageUrl

            };

            ViewBag.Users = new List<object>();
            foreach (var item in users)
            {
                ViewBag.Users.Add(new
                {
                    Id = item.Id,
                    Email = item.Email,
                    Username = item.UserName,
                    ImageUrl = item.ImageUrl

                });
            }

            var currentUserViewModel = new UserViewModel
            {
                Id = current.Id,
                Username = current.UserName,
                ImageUrl = current.ImageUrl

            };
            var userViewModels = users.Select(user => new UserViewModel
            {
                Id = user.Id,
                Username = user.UserName,
                ImageUrl = user.ImageUrl

            }).ToList();
            var chatViewModel = new ChatViewModel
            {
                CurrentUser = currentUserViewModel,
                Users = userViewModels
            };
            return View("LiveChat", chatViewModel);
        }

        public async Task<IActionResult> AddMessage([FromBody] MessageViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var chat = await _dbContext.Chats.FirstOrDefaultAsync(c =>
                    (c.SenderId == model.SenderId && c.ReceiverId == model.ReceiverId)
                    || (c.SenderId == model.ReceiverId && c.ReceiverId == model.SenderId));

                if (chat == null)
                {
                    chat = new Chat
                    {
                        SenderId = model.SenderId,
                        ReceiverId = model.ReceiverId
                    };
                    await _dbContext.Chats.AddAsync(chat);
                    await _dbContext.SaveChangesAsync();
                }

                var message = new Message
                {
                    ChatId = chat.Id,
                    Content = model.Content,
                    DateTime = DateTime.Now,
                    HasSeen = false,
                    IsImage = false,
                    ReceiverId = model.ReceiverId,
                    SenderId = model.SenderId,
                };

                await _dbContext.Messages.AddAsync(message);
                await _dbContext.SaveChangesAsync();

                // Send the message to the receiver via SignalR
                await _chatHubContext.Clients.User(model.ReceiverId).SendAsync("ReceiveMessage", model.SenderId, model.Content);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok();
        }

        private async Task<List<Message>> GetChatHistoryInternal(string senderId, string receiverId)
        {
            return await _dbContext.Messages
                .Where(m => (m.SenderId == senderId && m.ReceiverId == receiverId) ||
                            (m.SenderId == receiverId && m.ReceiverId == senderId))
                .OrderBy(m => m.DateTime)
                .ToListAsync();
        }
    }
}
