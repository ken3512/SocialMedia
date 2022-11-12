using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SocialMedia.Data;
using SocialMedia.Dtos.Post;

namespace SocialMedia.Services.PostService
{
    public class PostService : IPostService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public PostService(IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _mapper = mapper;
        }

        private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext.User
            .FindFirstValue(ClaimTypes.NameIdentifier));

        public async Task<ServiceResponse<List<GetPostDto>>> CreatePost(CreatePostDto newPost)
        {
            var response = new ServiceResponse<List<GetPostDto>>();

            try
            {
                Post post = _mapper.Map<Post>(newPost);
                post.User = await _context.Users.FirstOrDefaultAsync(u => u.Id == GetUserId());
                _context.Posts.Add(post);
                await _context.SaveChangesAsync();

                var dbPosts = await _context.Posts
                    .Where(p => p.UserId == GetUserId())
                    .ToListAsync();

                response.Data = dbPosts.Select(p => _mapper.Map<GetPostDto>(p)).ToList();
            }
            catch(Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<List<GetPostDto>>> GetPosts()
        {
            var response = new ServiceResponse<List<GetPostDto>>();

            try
            {
                var dbPosts = await _context.Posts
                    .Include(p => p.Likes)
                    .Where(p => p.UserId == GetUserId())
                    .ToListAsync();

                response.Data = dbPosts.Select(p => _mapper.Map<GetPostDto>(p)).ToList();
            }
            catch(Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<List<GetPostDto>>> DeletePost(int id)
        {
            var response = new ServiceResponse<List<GetPostDto>>();

            try
            {
                Post post = await _context.Posts
                    .Include(p => p.Likes)
                    .FirstOrDefaultAsync(p => p.Id == id && p.UserId == GetUserId());
                
                if(post == null)
                {
                    response.Success = false;
                    response.Message = "Post not found.";
                    return response;
                }

                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();

                var dbPosts = await _context.Posts
                    .Include(p => p.Likes)
                    .Where(p => p.UserId == GetUserId())
                    .ToListAsync();

                response.Data = dbPosts.Select(p => _mapper.Map<GetPostDto>(p)).ToList();
            }
            catch(Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<List<GetPostDto>>> EditPost(UpdatePostDto updatedPost)
        {
            var response = new ServiceResponse<List<GetPostDto>>();

            try
            {
                Post post = await _context.Posts
                    .Include(p => p.Likes)
                    .FirstOrDefaultAsync(p => p.Id == updatedPost.Id && p.UserId == GetUserId());
                
                if(post == null)
                {
                    response.Success = false;
                    response.Message = "Post not found.";
                    return response;
                } else if(
                    (updatedPost.Title.Equals(string.Empty) && updatedPost.Content.Equals(string.Empty)) || 
                    (updatedPost.Title.Equals(post.Title) && updatedPost.Content.Equals(post.Content)))
                {
                    response.Success = false;
                    response.Message = "No Changes to apply.";
                    return response;
                }

                if(updatedPost.Title != string.Empty)
                {
                    post.Title = updatedPost.Title;
                }

                if(updatedPost.Content != string.Empty)
                {
                    post.Content = updatedPost.Content;
                }

                await _context.SaveChangesAsync();

                var dbPosts = await _context.Posts
                    .Include(p => p.Likes)
                    .Where(p => p.UserId == GetUserId())
                    .ToListAsync();

                response.Data = dbPosts.Select(p => _mapper.Map<GetPostDto>(p)).ToList();
            }
            catch(Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        // TODO:
        // Fix after relationships are implemented
        public async Task<ServiceResponse<GetPostDto>> LikePost(int id)
        {
            var response = new ServiceResponse<GetPostDto>();

            try
            {
                Post post = await _context.Posts
                    .Include(p => p.Likes)
                    .FirstOrDefaultAsync(p => p.Id == id);

                User user = await _context.Users.FirstOrDefaultAsync(u => u.Id == GetUserId());
                
                if(post == null)
                {
                    response.Success = false;
                    response.Message = "Post not found.";
                    return response;
                }
                else if(post.Likes.Contains(user))
                {
                    response.Success = false;
                    response.Message = "Post already liked.";
                    return response;
                }

                post.Likes.Add(user);
                await _context.SaveChangesAsync();

                response.Data = _mapper.Map<GetPostDto>(post);
            }
            catch(Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }
    }
}