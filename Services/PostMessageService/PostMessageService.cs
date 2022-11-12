using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SocialMedia.Data;
using SocialMedia.Dtos.Post;
using SocialMedia.Dtos.PostMessage;

namespace SocialMedia.Services.PostMessageService
{
    public class PostMessageService : IPostMessageService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public PostMessageService(IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _mapper = mapper;
            
        }

        private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext.User
            .FindFirstValue(ClaimTypes.NameIdentifier));

        public async Task<ServiceResponse<GetPostDto>> DeletePostMessage(int id)
        {
            var response = new ServiceResponse<GetPostDto>();

            try
            {
                PostMessage message = await _context.Messages
                    .FirstOrDefaultAsync(p => p.Id == id && p.UserId == GetUserId());
                
                if(message == null)
                {
                    response.Success = false;
                    response.Message = "Message not found.";
                    return response;
                }

                Post post = await _context.Posts
                    .Include(p => p.Messages).ThenInclude(m => m.User)
                    .Include(p => p.Likes)
                    .Include(p => p.User)
                    .FirstOrDefaultAsync(p => p.Id == message.PostId);

                _context.Messages.Remove(message);
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

        public async Task<ServiceResponse<GetPostDto>> SendPostMessage(CreatePostMessageDto newMessage)
        {
            var response = new ServiceResponse<GetPostDto>();

            try
            {
                Post post = await _context.Posts
                    .Include(p => p.Messages).ThenInclude(m => m.User)
                    .Include(p => p.Likes)
                    .Include(p => p.User)
                    .Include(p => p.User).ThenInclude(u => u.Received)
                    .Include(p => p.User).ThenInclude(u => u.Sent)
                    .FirstOrDefaultAsync(p => p.Id == newMessage.PostId);

                if(post == null)
                {
                    response.Success = false;
                    response.Message = "Post not found.";
                    return response;
                }

                User author = post.User;
                User currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == GetUserId());

                if(author.Received.Any(r => r.SenderId == GetUserId()) || author.Sent.Any(r => r.ReceiverId == GetUserId()))
                {
                    PostMessage message = _mapper.Map<PostMessage>(newMessage);
                    message.User = currentUser;
                    message.Post = post;
                    _context.Messages.Add(message);
                    await _context.SaveChangesAsync();

                    response.Data = _mapper.Map<GetPostDto>(post);
                } 
                else
                {
                    response.Success = false;
                    response.Message = "Post not found.";
                }
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