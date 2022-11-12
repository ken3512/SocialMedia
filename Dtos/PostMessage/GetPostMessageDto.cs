using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SocialMedia.Dtos.User;

namespace SocialMedia.Dtos.PostMessage
{
    public class GetPostMessageDto
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public GetUserDto User { get; set; }
    }
}