using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialMedia.models
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public List<User> Likes { get; set; }
        public User User { get; set; }
        public int UserId { get; set; }
        public List<PostMessage> Messages { get; set; }
    }
}