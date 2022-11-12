using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SocialMedia.Dtos.PostMessage;

namespace SocialMedia.Dtos.Post
{
    public class GetPostDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int Likes { get; set; }
        public List<GetPostMessageDto> Messages { get; set; }
    }
}