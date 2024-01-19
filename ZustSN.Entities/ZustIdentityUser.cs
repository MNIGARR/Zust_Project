using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZustSN.Core.Abstraction;

namespace ZustSN.Entities
{
    public class ZustIdentityUser: IdentityUser, IEntity
    {
        public bool IsFriend { get; set; }
        public bool IsOnline { get; set; }
        public DateTime DisconnectTime { get; set; } = DateTime.Now;
        public string ConnectTime { get; set; } = "";
        public string? ImageUrl { get; set; }
        public bool HasRequestPending { get; set; }
        public virtual ICollection<Chat>? Chats { get; set; }
        public List<Friend>? Friends { get; set; } = new List<Friend>();
        public List<FriendRequest> FriendRequests { get; set; } = new List<FriendRequest>();
        public List<Message> Messages { get; set; } = new List<Message>();
        public List<Post>? Posts { get; set; }
        public ZustIdentityUser()
        {
        }

    }
}
