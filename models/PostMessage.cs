using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialMedia.models
{
    public class PostMessage
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public User User { get; set; }
        public int UserId { get; set; }
        public Post Post { get; set; }
        public int PostId { get; set; }
    }
}