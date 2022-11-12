using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SocialMedia.Data;
using SocialMedia.Dtos.Post;
using SocialMedia.Dtos.User;

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

        public async Task<ServiceResponse<List<GetPostDto>>> GetPosts(int id)
        {
            var response = new ServiceResponse<List<GetPostDto>>();

            try
            {
                Relationship check = await _context.Relationships
                    .FirstOrDefaultAsync(r => (r.SenderId == id && r.ReceiverId == GetUserId()) ||
                    (r.SenderId == GetUserId() && r.ReceiverId == id) &&
                    r.pending == false);
                if(check == null && id != GetUserId())
                {
                    response.Success = false;
                    response.Message = "Must be friends to see posts.";
                    return response;
                }

                var dbPosts = await _context.Posts
                    .Include(p => p.Likes)
                    .Where(p => p.UserId == id)
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

        public async Task<ServiceResponse<GetPostDto>> LikePost(int id)
        {
            var response = new ServiceResponse<GetPostDto>();

            try
            {
                Post post = await _context.Posts
                    .Include(p => p.Likes)
                    .Include(p => p.User)
                    .FirstOrDefaultAsync(p => p.Id == id);
                
                if(post == null)
                {
                    response.Success = false;
                    response.Message = "Post not found.";
                    return response;
                }

                User user = await _context.Users.FirstOrDefaultAsync(u => u.Id == GetUserId());
                
                Relationship check = await _context.Relationships
                    .FirstOrDefaultAsync(r => (r.SenderId == post.User.Id && r.ReceiverId == GetUserId()) ||
                    (r.SenderId == GetUserId() && r.ReceiverId == post.User.Id) &&
                    r.pending == false);
                
                if(check == null && post.User.Id != GetUserId())
                {
                    response.Success = false;
                    response.Message = "Post not found.";
                    return response;
                }
                else if(post.User.Id == GetUserId())
                {
                    response.Success = false;
                    response.Message = "Cannot like your own post.";
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

        public async Task<ServiceResponse<List<GetPostDto>>> GetFeed()
        {
            var response = new ServiceResponse<List<GetPostDto>>();

            try
            {
                List<User> friends = await _context.Users
                    .Where(u => u.Received.Any(r => r.pending == false && r.SenderId == GetUserId()) ||
                    u.Sent.Any(r => r.pending == false && r.ReceiverId == GetUserId()))
                    .ToListAsync();
                
                List<Post> posts = await _context.Posts
                    .Where(p => friends.Contains(p.User))
                    .ToListAsync();
                
                response.Data = posts.Select(p => _mapper.Map<GetPostDto>(p)).ToList();
            }
            catch(Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<List<GetUserDto>>> GetFriends()
        {
            var response = new ServiceResponse<List<GetUserDto>>();

            try
            {
                List<User> friends = await _context.Users
                    .Where(u => u.Received.Any(r => r.pending == false && r.SenderId == GetUserId()) ||
                    u.Sent.Any(r => r.pending == false && r.ReceiverId == GetUserId()))
                    .ToListAsync();
                
                response.Data = friends.Select(f => _mapper.Map<GetUserDto>(f)).ToList();
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