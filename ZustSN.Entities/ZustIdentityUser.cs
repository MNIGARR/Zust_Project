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
    }
}
