using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Serialization;
using ZustSN.Entities;

namespace ZustSN.WebUI.Controllers
{
    public class ProfileController : Controller
    {
        private UserManager<ZustIdentityUser> _userManager;
        private ZustIdentityDBContext _dbContext;

        public ProfileController(UserManager<ZustIdentityUser> userManager, ZustIdentityDBContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
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

        public async Task<IActionResult> FriendRequests()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var friendrequsts = _dbContext.FriendRequests?
                .Where(friend => friend.ReceiverId == user.Id && friend.Status == "Pending")
                .Include(friend => friend.Sender)
                .ToList();

            ViewBag.User = new
            {
                ImageUrl = user.ImageUrl,
                Username = user.UserName,
                Email = user.Email

            };

            ViewBag.FriendRequsts = friendrequsts?
                .Select(friend => new
                {
                    SenderName = friend.Sender?.UserName,
                    SenderEmail = friend.Sender?.Email,
                    SenderImageUrl = friend.Sender?.ImageUrl,

                });
            return RedirectToAction("Friend", "Profile");
        }


        public async Task<IActionResult> SendFriendRequst(string id)
        {
            var sender = await _userManager.GetUserAsync(HttpContext.User);
            var receiverUser = _userManager.Users.FirstOrDefault(user => user.Id == id);
            if (receiverUser != null)
            {
                _dbContext.FriendRequests?.Add( new FriendRequest{
                    SenderId= sender.Id,
                    Sender = sender,
                    ReceiverId = id,
                    Status="Pending"
                });
                receiverUser.HasRequestPending = true;
                await _dbContext.SaveChangesAsync();
                await _userManager.UpdateAsync(receiverUser);
                return Ok();
            }
            return BadRequest();
        }


        public async Task<IActionResult> AcceptFollowRequest(string id)
        {
            var receiver = await _userManager.GetUserAsync(HttpContext.User);
            var sender = _userManager.Users.FirstOrDefault(user => user.Id == id);
            var requestId = _dbContext.FriendRequests
                .FirstOrDefault(friend => friend.ReceiverId == receiver.Id && friend.SenderId == sender.Id)
                .Id;
            if (receiver != null)
            {
                receiver.FriendRequests.Add(new FriendRequest
                {
                    SenderId = sender?.Id,
                    Sender = sender,
                    ReceiverId = receiver.Id,
                    Status = "Notification"
                });
                var receiverUser = new Friend
                {
                    OwnId = receiver.Id,
                    YourFriendId = sender?.Id,
                };
                var senderUser = new Friend
                {
                    OwnId = sender?.Id,
                    YourFriendId = receiver.Id,
                };
                _dbContext.Friends?.Add(senderUser);
                _dbContext.Friends.Add(receiverUser);

                var friendRequest = await _dbContext.FriendRequests
                    .FirstOrDefaultAsync(request => request.Id == requestId);

                _dbContext.FriendRequests.Remove(friendRequest);
                await _userManager.UpdateAsync(receiver);
                await _userManager.UpdateAsync(sender);
                await _dbContext.SaveChangesAsync();
            }
            return RedirectToAction("Friends", "Profile");
        }

        public async Task<IActionResult> UnFollow(string id)
        {
            try
            {
                var current = await _userManager.GetUserAsync(HttpContext.User);
                var friendItems = _dbContext.Friends?.Where(f => f.OwnId == id && f.YourFriendId == current.Id || f.YourFriendId == id && f.OwnId == current.Id);
                _dbContext.Friends?.RemoveRange(friendItems);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction("Friends", "Profile");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public async Task<IActionResult> DeclineRequest(int id, string senderId)
        {
            try
            {
                var current = await _userManager.GetUserAsync(HttpContext.User);
                var friendRequest = await _dbContext.FriendRequests.FirstOrDefaultAsync(friend => friend.Id == id);
                var sender = await _dbContext.Users.FirstOrDefaultAsync(user => user.Id == senderId);
                _dbContext.FriendRequests.Add(new FriendRequest
                {
                    SenderId = current.Id,
                    Sender = current,
                    ReceiverId = sender?.Id,
                    Status = "Notification"
                });

                _dbContext.FriendRequests.Remove(friendRequest);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction("Friends", "Profile");
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public async Task<IActionResult> Followers()
        {
            var follower = await _userManager.GetUserAsync(HttpContext.User);
            ViewBag.User = new
            {
                Email = follower.Email,
                UserName = follower.UserName,
                ImageUrl = follower.ImageUrl
            };
            return View("Followers");
        }

        public async Task<IActionResult> Users()
        {
            var current = await _userManager.GetUserAsync(HttpContext.User);
            var friends = await _dbContext.Users.Where(friend => friend.Id != current.Id).ToListAsync();
            var status = " ";
            var isPending = false;
            var isDisabled = false;
            ViewBag.User = new
            {
                Email = current.Email,
                UserName = current.UserName,
                ImageUrl = current.ImageUrl
            };
            var sentRequests = _dbContext.FriendRequests.Where(request => request.SenderId == current.Id && request.Status == "Pending").ToList();
            var myRequests = _dbContext.FriendRequests.Where(request => request.ReceiverId == current.Id && request.Status == "Pending").ToList();

            ViewBag.Users = new List<object>();
            foreach (var item in friends)
            {
                var myRequest = myRequests.FirstOrDefault(request => request.SenderId == item.Id);
                var sentRequest = sentRequests.FirstOrDefault(request => request.ReceiverId == item.Id);

                bool isPendingSentRequest = sentRequest != null;
                bool isPendingMyRequest = myRequest != null;
                var friendship = _dbContext.Friends?.FirstOrDefault(f =>
                    (f.OwnId == current.Id && f.YourFriendId == item.Id) ||
                    (f.OwnId == item.Id && f.YourFriendId == current.Id));

                bool isFriend = friendship != null;
                if (isPendingSentRequest)
                {
                    item.HasRequestPending = true;
                    status = "Pending";
                    isDisabled = true;
                }
                else if (isPendingMyRequest)
                {
                    isPending = true;
                }
                else
                {
                    status = "Add Friend";
                }
                ViewBag.Users.Add(new
                {
                    Id = item.Id,
                    Username = item.UserName,
                    Email = item.Email,
                    ImageUrl = item.ImageUrl,
                    Status = status,
                    IsFriend = isFriend,
                    IsButtonDisabled = isDisabled,
                    IsPending = isPending
                });
            }
            return View("Friends");
        }

        public async Task<IActionResult> Notifications()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            ViewBag.User = new
            {
                Email = currentUser.Email,
                Username = currentUser.UserName,
                ImageUrl = currentUser.ImageUrl

            };
            return View("Notifications");
        }

        public async Task<IActionResult> Privacy()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            ViewBag.User = new
            {
                Email = currentUser.Email,
                Username = currentUser.UserName,
                ImageUrl = currentUser.ImageUrl

            };
            return View("Privacy");
        }

        public async Task<IActionResult> Help()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            ViewBag.User = new
            {
                Email = currentUser.Email,
                Username = currentUser.UserName,
                ImageUrl = currentUser.ImageUrl

            };
            return View("Help");
        }     
    }
}
