using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SocialMedia.Dtos.Post;
using SocialMedia.Dtos.User;

namespace SocialMedia.Services.PostService
{
    public interface IPostService
    {
        Task<ServiceResponse<List<GetPostDto>>> CreatePost(CreatePostDto newPost);
        Task<ServiceResponse<List<GetPostDto>>> GetPosts(int id);
        Task<ServiceResponse<List<GetPostDto>>> DeletePost(int id);
        Task<ServiceResponse<List<GetPostDto>>> EditPost(UpdatePostDto updatedPost);
        Task<ServiceResponse<GetPostDto>> LikePost(int id);
        Task<ServiceResponse<List<GetPostDto>>> GetFeed();
        Task<ServiceResponse<List<GetUserDto>>> GetFriends();

    }
}