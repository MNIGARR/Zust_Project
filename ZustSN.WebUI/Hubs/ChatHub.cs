using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using ZustSN.Entities;
namespace ZustSN.WebUI.Hubs
{
    public class ChatHub: Hub
    {
        private UserManager<ZustIdentityUser> _userManager;
        private ZustIdentityDBContext _context;
        private IHttpContextAccessor _contextAccessor;
        public ChatHub(UserManager<ZustIdentityUser> userManager, IHttpContextAccessor contextAccessor, ZustIdentityDBContext context)
        {
            _userManager = userManager;
            _contextAccessor = contextAccessor;
            _context = context;
        }
        public async Task SendMessage(string receiverId, string senderId, string message)
        {
            await Clients.User(receiverId).SendAsync("ReceiveMessage", senderId, message);
        }
        public async override Task OnConnectedAsync()
        {

            var user = await _userManager.GetUserAsync(_contextAccessor.HttpContext.User);
            var userItem = _context.Users.SingleOrDefault(x => x.Id == user.Id);
            userItem.IsOnline = true;
            await _context.SaveChangesAsync();

            string info = user.UserName + " connected successfully";
            await Clients.Others.SendAsync("Connect", "Yes");
        }
        public async override Task OnDisconnectedAsync(Exception? exception)
        {
            var user = await _userManager.GetUserAsync(_contextAccessor.HttpContext.User);
            var userItem = _context.Users.SingleOrDefault(x => x.Id == user.Id);
            userItem.IsOnline = false;
            userItem.DisconnectTime = DateTime.Now;
            await _context.SaveChangesAsync();
            string info = user.UserName + " disconnected successfully";
            await Clients.Others.SendAsync("Disconnect", info);
        }
    }
}
