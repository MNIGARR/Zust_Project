using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZustSN.Entities
{
    public class Post
    {
        public int Id { get; set; }
        public List<Image>? ImageUrls { get; set; }
        public List<Video>? VideoUrls { get; set; }
        public List<Friend>? TaggedFriends { get; set; }
        public DateTime PostDate { get; set; }
    }
}
