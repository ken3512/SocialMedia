using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Dtos.Post;
using SocialMedia.Dtos.PostMessage;
using SocialMedia.Services.PostMessageService;

namespace SocialMedia.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PostMessageController : ControllerBase
    {
        private readonly IPostMessageService _postMessageService;
        public PostMessageController(IPostMessageService postMessageService)
        {
            _postMessageService = postMessageService;
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<GetPostDto>>> SendPostMessage(CreatePostMessageDto newMessage)
        {
            var response = await _postMessageService.SendPostMessage(newMessage);
            if(response.Success == false)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpDelete]
        public async Task<ActionResult<ServiceResponse<GetPostDto>>> DeletePostMessage(int id)
        {
            var response = await _postMessageService.DeletePostMessage(id);
            if(response.Success == false)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}