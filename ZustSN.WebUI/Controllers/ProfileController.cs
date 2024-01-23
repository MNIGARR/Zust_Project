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
        private ZustIdentityDBContext _context;

        public ProfileController(UserManager<ZustIdentityUser> userManager, ZustIdentityDBContext context)
        {
            _userManager = userManager;
            _context = context;
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

            var friendRequests = _context.FriendRequests?
                .Where(fr => fr.ReceiverId == user.Id && fr.Status == "Pending")
                .Include(fr => fr.Sender)
                .ToList();

            ViewBag.User = new
            {
                ImageUrl = user.ImageUrl,
                Username = user.UserName,
                Email = user.Email
            };
            ViewBag.FriendRequests = friendRequests?.Select(fr => new
            {
                SenderName = fr.Sender?.UserName,
                SenderEmail = fr.Sender?.Email,
                SenderImage = fr.Sender?.ImageUrl
            });

            return RedirectToAction("Friends", "Profile");
        }
        public async Task<IActionResult> SendFriendRequest(string id)
        {
            var sender = await _userManager.GetUserAsync(HttpContext.User);
            var receiverUser = _userManager.Users.FirstOrDefault(u => u.Id == id);
            if (receiverUser != null)
            {
                _context.FriendRequests?.Add(new FriendRequest
                {
                    //Content = $"{sender.UserName} send friend request at {DateTime.Now.ToLongDateString()}",
                    SenderId = sender.Id,
                    Sender = sender,
                    ReceiverId = id,
                    Status = "Pending"

                });
                receiverUser.HasRequestPending = true;
                await _context.SaveChangesAsync();
                await _userManager.UpdateAsync(receiverUser);
                return Ok();
            }
            return BadRequest();
        }
        public async Task<IActionResult> AcceptRequest(string id)
        {
            var receiver = await _userManager.GetUserAsync(HttpContext.User);
            var sender = _userManager.Users.FirstOrDefault(u => u.Id == id);
            var requestId = _context.FriendRequests.FirstOrDefault(f => f.ReceiverId == receiver.Id && f.SenderId == sender.Id).Id;
            if (receiver != null)
            {
                receiver.FriendRequests.Add(new FriendRequest
                {
                    SenderId = sender?.Id,
                    Sender = sender,
                    ReceiverId = receiver.Id,
                    Status = "Notification"
                });

                var receiverFriend = new Friend
                {
                    OwnId = receiver.Id,
                    YourFriendId = sender?.Id,
                };

                var senderFriend = new Friend
                {
                    OwnId = sender?.Id,
                    YourFriendId = receiver.Id,
                };

                _context.Friends?.Add(senderFriend);
                _context.Friends.Add(receiverFriend);

                var request = await _context.FriendRequests.FirstOrDefaultAsync(r => r.Id == requestId);
                _context.FriendRequests.Remove(request);

                await _userManager.UpdateAsync(receiver);
                await _userManager.UpdateAsync(sender);


                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Friends", "Profile");
        }

        public async Task<IActionResult> DeleteRequest(int id, string senderId)
        {
            try
            {
                var current = await _userManager.GetUserAsync(HttpContext.User);
                var request = await _context.FriendRequests.FirstOrDefaultAsync(f => f.Id == id);
                var sender = await _context.Users.FirstOrDefaultAsync(u => u.Id == senderId);

                _context.FriendRequests.Add(new FriendRequest
                {
                    //Content = $"${current.UserName} declined your friend request at {DateTime.Now.ToLongTimeString()}",
                    SenderId = current.Id,
                    Sender = current,
                    ReceiverId = sender?.Id,
                    Status = "Notification"
                });


                _context.FriendRequests.Remove(request);
                await _context.SaveChangesAsync();
                return RedirectToAction("Friends", "Profile");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        public async Task<IActionResult> UnFollow(string id)
        {
            try
            {
                var current = await _userManager.GetUserAsync(HttpContext.User);
                var friendItems = _context.Friends?.Where(f => f.OwnId == id && f.YourFriendId == current.Id || f.YourFriendId == id && f.OwnId == current.Id);
                _context.Friends?.RemoveRange(friendItems);
                await _context.SaveChangesAsync();
                return RedirectToAction("Friends", "Profile");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        public async Task<IActionResult> Setting()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            ViewBag.User = new
            {
                ImageUrl = user.ImageUrl,
                Username = user.UserName,
                Email = user.Email
            };
            return View("Setting");
        }
        public async Task<IActionResult> Followers()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            ViewBag.User = new
            {
                ImageUrl = user.ImageUrl,
                Username = user.UserName,
                Email = user.Email
            };
            return View("Followers");
        }
        public async Task<IActionResult> Privacy()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            ViewBag.User = new
            {
                ImageUrl = user.ImageUrl,
                Username = user.UserName,
                Email = user.Email
            };
            return View("Privacy");
        }
        public async Task<IActionResult> Friends()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var users = await _context.Users
                .Where(u => u.Id != currentUser.Id)
                .ToListAsync();
            var status = "";
            var isDisabled = false;
            var isPending = false;
            ViewBag.User = new
            {
                ImageUrl = currentUser.ImageUrl,
                Username = currentUser.UserName,
                Email = currentUser.Email
            };

            var myRequestsToOthers = _context.FriendRequests
                .Where(r => r.SenderId == currentUser.Id && r.Status == "Pending")
                .ToList();

            var friendRequestsToMe = _context.FriendRequests
                .Where(r => r.ReceiverId == currentUser.Id && r.Status == "Pending")
                .ToList();

            ViewBag.Users = new List<object>();

            foreach (var item in users)
            {
                var requestToOther = myRequestsToOthers.FirstOrDefault(r => r.ReceiverId == item.Id);
                var requestToMe = friendRequestsToMe.FirstOrDefault(r => r.SenderId == item.Id);

                bool isPendingToOther = requestToOther != null;
                bool isPendingToMe = requestToMe != null;
                var friendship = _context.Friends?.FirstOrDefault(f =>
                    (f.OwnId == currentUser.Id && f.YourFriendId == item.Id) ||
                    (f.OwnId == item.Id && f.YourFriendId == currentUser.Id));

                bool isFriend = friendship != null;
                if (isPendingToOther)
                {
                    item.HasRequestPending = true;
                    status = "Pending";
                    isDisabled = true;
                }
                else if (isPendingToMe)
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
                    ImageUrl = item.ImageUrl,
                    Username = item.UserName,
                    Email = item.Email,
                    Status = status,
                    IsButtonDisabled = isDisabled,
                    IsPending = isPending,
                    IsFriend = isFriend
                });
            }

            return View("Friends");
        }

        public async Task<IActionResult> HelpAndSupport()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            ViewBag.User = new
            {
                ImageUrl = user.ImageUrl,
                Username = user.UserName,
                Email = user.Email
            };
            return View("HelpAndSupport");
        }
        public async Task<IActionResult> Notifications()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            ViewBag.User = new
            {
                ImageUrl = user.ImageUrl,
                Username = user.UserName,
                Email = user.Email
            };
            return View("Notifications");
        }
    }
}
