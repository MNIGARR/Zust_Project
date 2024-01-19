using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZustSN.Entities
{
    public class ZustIdentityDBContext : IdentityDbContext<ZustIdentityUser, ZustIdentityRole, string>
    {
        public ZustIdentityDBContext(DbContextOptions<ZustIdentityDBContext> options)
            : base(options)
        {
        }
        public DbSet<Chat>? Chats { get; set; }
        public DbSet<Friend>? Friends { get; set; }
        public DbSet<FriendRequest>? FriendRequests { get; set; }
        public DbSet<Message>? Messages { get; set; }
        public DbSet<Post>? Posts { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Friend>().HasKey(f => f.Id); 
        }
    }
}
