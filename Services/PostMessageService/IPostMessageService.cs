using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SocialMedia.Dtos.Post;
using SocialMedia.Dtos.PostMessage;

namespace SocialMedia.Services.PostMessageService
{
    public interface IPostMessageService
    {
        Task<ServiceResponse<GetPostDto>> SendPostMessage(CreatePostMessageDto newMessage);
        Task<ServiceResponse<GetPostDto>> DeletePostMessage(int id);
    }
}