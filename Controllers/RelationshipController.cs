using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Dtos.Post;
using SocialMedia.Dtos.Relationship;
using SocialMedia.Dtos.User;
using SocialMedia.Services.RelationshipService;

namespace SocialMedia.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class RelationshipController : ControllerBase
    {
        private readonly IRelationshipService _relationshipService;
        public RelationshipController(IRelationshipService relationshipService)
        {
            _relationshipService = relationshipService;
        }

        [HttpGet("Sent")]
        public async Task<ActionResult<ServiceResponse<List<GetRelationshipDto>>>> GetSent() 
        {
            var response = await _relationshipService.GetSent();
            if(response.Data == null)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpGet("Received")]
        public async Task<ActionResult<ServiceResponse<List<GetRelationshipDto>>>> GetReceived() 
        {
            var response = await _relationshipService.GetReceived();
            if(response.Data == null)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("Send")]
        public async Task<ActionResult<ServiceResponse<GetPostDto>>> SendRequest(int userId) 
        {
            var response = await _relationshipService.SendRequest(userId);
            if(response.Data == null)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpDelete("Reject")]
        public async Task<ActionResult<ServiceResponse<GetPostDto>>> RejectRequest(int userId) 
        {
            var response = await _relationshipService.RejectRequest(userId);
            if(response.Data == null)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpDelete("Cancel")]
        public async Task<ActionResult<ServiceResponse<GetUserDto>>> CancelRequest(int userId) 
        {
            var response = await _relationshipService.CancelRequest(userId);
            if(response.Data == null)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPut("Accept")]
        public async Task<ActionResult<ServiceResponse<GetRelationshipDto>>> AcceptRequest(int userId) 
        {
            var response = await _relationshipService.AcceptRequest(userId);
            if(response.Data == null)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }



        
    }
}