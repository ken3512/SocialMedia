using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialMedia.models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public List<Post> Posts { get; set; }
        public List<Post> Liked { get; set; }
        public List<Relationship> Sent { get; set; }
        public List<Relationship> Received { get; set; }
        public List<PostMessage> Messages { get; set; }
    }
}