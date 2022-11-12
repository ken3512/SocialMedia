using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Dtos.Post;
using SocialMedia.Dtos.User;
using SocialMedia.Services.PostService;

namespace SocialMedia.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        public PostController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse<List<GetPostDto>>>> GetAllPosts(int id)
        {
            var response = await _postService.GetPosts(id);
            if(response.Data == null)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<GetPostDto>>>> GetFeed()
        {
            var response = await _postService.GetFeed();
            if(response.Data == null)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpGet("Friends")]
        public async Task<ActionResult<ServiceResponse<List<GetUserDto>>>> GetFriends()
        {
            var response = await _postService.GetFriends();
            if(response.Data == null)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<List<GetPostDto>>>> CreatePost(CreatePostDto newPost)
        {
            var response = await _postService.CreatePost(newPost);
            if(response.Data == null)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpDelete]
        public async Task<ActionResult<ServiceResponse<List<GetPostDto>>>> DeletePost(int id)
        {
            var response = await _postService.DeletePost(id);
            if(response.Data == null)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPut]
        public async Task<ActionResult<ServiceResponse<List<GetPostDto>>>> EditPost(UpdatePostDto updatedPost)
        {
            var response = await _postService.EditPost(updatedPost);
            if(response.Data == null)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPut("Like")]
        public async Task<ActionResult<ServiceResponse<GetPostDto>>> LikePost(int id)
        {
            var response = await _postService.LikePost(id);
            if(response.Data == null)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

    }
}