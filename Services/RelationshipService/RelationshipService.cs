using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SocialMedia.Data;
using SocialMedia.Dtos.Relationship;
using SocialMedia.Dtos.User;

namespace SocialMedia.Services.RelationshipService
{
    public class RelationshipService : IRelationshipService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public RelationshipService(IMapper mapper, DataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _mapper = mapper;
            
        }

        private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext.User
            .FindFirstValue(ClaimTypes.NameIdentifier));


        public async Task<ServiceResponse<GetRelationshipDto>> AcceptRequest(int id)
        {
            var response = new ServiceResponse<GetRelationshipDto>();

            try
            {
                Relationship relationship = await _context.Relationships
                    .Include(r => r.Receiver)
                    .Include(r => r.Sender)
                    .FirstOrDefaultAsync(r => r.SenderId == id && 
                    r.ReceiverId == GetUserId());

                if(relationship == null)
                {
                    response.Success = false;
                    response.Message = "Friend request not found.";
                    return response;
                }
                else if(relationship.pending == false)
                {
                    response.Success = false;
                    response.Message = $"You are already friends with {relationship.Sender.Username}.";
                    return response;
                }

                relationship.pending = false;
                await _context.SaveChangesAsync();
                response.Data = _mapper.Map<GetRelationshipDto>(relationship);
            }
            catch(Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<GetUserDto>> CancelRequest(int id)
        {
            var response = new ServiceResponse<GetUserDto>();

            try
            {
                Relationship relationship = await _context.Relationships
                    .Include(r => r.Receiver)
                    .Include(r => r.Sender)
                    .FirstOrDefaultAsync(r => r.SenderId == GetUserId() && 
                    r.ReceiverId == id);

                if(relationship == null)
                {
                    response.Success = false;
                    response.Message = "Friend request not found.";
                    return response;
                }
                else if(relationship.pending == false)
                {
                    response.Success = false;
                    response.Message = $"You are friends with {relationship.Sender.Username}.";
                    return response;
                }

                User receiver = relationship.Receiver;

                _context.Relationships.Remove(relationship);
                await _context.SaveChangesAsync();
                response.Data = _mapper.Map<GetUserDto>(receiver);
            }
            catch(Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<List<GetRelationshipDto>>> GetReceived()
        {
            var response = new ServiceResponse<List<GetRelationshipDto>>();

            try
            {
                List<Relationship> relationships = await _context.Relationships
                    .Include(r => r.Receiver)
                    .Include(r => r.Sender)
                    .Where(r => r.ReceiverId == GetUserId()  && r.pending == true)
                    .ToListAsync();

                response.Data = relationships
                    .Select(r => _mapper.Map<GetRelationshipDto>(r))
                    .ToList();
            }
            catch(Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<List<GetRelationshipDto>>> GetSent()
        {
            var response = new ServiceResponse<List<GetRelationshipDto>>();

            try
            {
                List<Relationship> relationships = await _context.Relationships
                    .Include(r => r.Receiver)
                    .Include(r => r.Sender)
                    .Where(r => r.SenderId == GetUserId()  && r.pending == true)
                    .ToListAsync();

                response.Data = relationships
                    .Select(r => _mapper.Map<GetRelationshipDto>(r))
                    .ToList();
            }
            catch(Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<GetUserDto>> RejectRequest(int id)
        {
            var response = new ServiceResponse<GetUserDto>();

            try
            {
                Relationship relationship = await _context.Relationships
                    .Include(r => r.Receiver)
                    .Include(r => r.Sender)
                    .FirstOrDefaultAsync(r => r.SenderId == id && 
                    r.ReceiverId == GetUserId());

                if(relationship == null)
                {
                    response.Success = false;
                    response.Message = "Friend request not found.";
                    return response;
                }
                else if(relationship.pending == false)
                {
                    response.Success = false;
                    response.Message = $"You are friends with {relationship.Sender.Username}.";
                    return response;
                }

                User sender = relationship.Sender;

                _context.Relationships.Remove(relationship);
                await _context.SaveChangesAsync();
                response.Data = _mapper.Map<GetUserDto>(sender);
            }
            catch(Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<GetRelationshipDto>> SendRequest(int id)
        {
            var response = new ServiceResponse<GetRelationshipDto>();

            try
            {
                Relationship check = await _context.Relationships
                    .Include(r => r.Receiver)
                    .Include(r => r.Sender)
                    .FirstOrDefaultAsync(r => (r.SenderId == id && r.ReceiverId == GetUserId()) ||
                    (r.SenderId == GetUserId() && r.ReceiverId == id));
                
                if(check != null)
                {

                    if(check.pending == false)
                    {
                        response.Success = false;
                        response.Message = $"You are already friends with {check.Sender.Username}.";
                        return response;
                    }
                    else if(check.pending == true && check.ReceiverId == GetUserId())
                    {
                        check.pending = false;
                        await _context.SaveChangesAsync();
                        response.Data = _mapper.Map<GetRelationshipDto>(check);
                        return response;
                    }
                    else
                    {
                        response.Success = false;
                        response.Message = $"You already sent a request to {check.Sender.Username}.";
                        return response;
                    }
                }
                else if(id == GetUserId())
                {
                    response.Success = false;
                    response.Message = "Cannot friend yourself.";
                    return response;
                }

                Relationship relationship = new Relationship
                {
                    Receiver =  await _context.Users.FirstOrDefaultAsync(u => u.Id == id),
                    Sender =  await _context.Users.FirstOrDefaultAsync(u => u.Id == GetUserId())
                };

                _context.Relationships.Add(relationship);
                await _context.SaveChangesAsync();

                response.Data = _mapper.Map<GetRelationshipDto>(relationship);
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