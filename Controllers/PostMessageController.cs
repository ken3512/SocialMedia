using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    }
}