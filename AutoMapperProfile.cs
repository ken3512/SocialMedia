using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using SocialMedia.Dtos.Post;
using SocialMedia.Dtos.PostMessage;
using SocialMedia.Dtos.User;

namespace SocialMedia
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Post, GetPostDto>();
            CreateMap<CreatePostDto, Post>();
            CreateMap<User, GetUserDto>();
            CreateMap<PostMessage, GetPostMessageDto>();
        }
    }
}