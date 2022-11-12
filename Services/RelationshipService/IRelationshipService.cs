using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SocialMedia.Dtos.Relationship;
using SocialMedia.Dtos.User;

namespace SocialMedia.Services.RelationshipService
{
    public interface IRelationshipService
    {
        Task<ServiceResponse<List<GetRelationshipDto>>> GetSent();
        Task<ServiceResponse<List<GetRelationshipDto>>> GetReceived();
        Task<ServiceResponse<GetRelationshipDto>> SendRequest(int id);
        Task<ServiceResponse<GetUserDto>> RejectRequest(int id);
        Task<ServiceResponse<GetUserDto>> CancelRequest(int id);
        Task<ServiceResponse<GetRelationshipDto>> AcceptRequest(int id);

    }
}