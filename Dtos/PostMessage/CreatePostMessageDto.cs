using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialMedia.Dtos.PostMessage
{
    public class CreatePostMessageDto
    {
        public string Text { get; set; } = string.Empty;
        public int PostId { get; set; }
    }
}