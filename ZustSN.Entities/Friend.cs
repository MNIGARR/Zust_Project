using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZustSN.Entities
{
    public class Friend
    {
        public int Id { get; set; }
        public string? OwnId { get; set; }
        public string? YourFriendId { get; set; }
        public virtual ZustIdentityUser? YourFriend { get; set; }
    }
}
