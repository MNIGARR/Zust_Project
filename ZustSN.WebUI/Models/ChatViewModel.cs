using ZustSN.Entities;

namespace ZustSN.WebUI.Models
{
    public class ChatViewModel
    {
        public Chat? CurrentChat { get; set; }
        public List<Chat>? Chats { get; set; }
        public string? CurrentUserId { get; set; }
        public UserViewModel? CurrentUser { get; set; }
        public List<UserViewModel>? Users { get; set; }
    }
}
