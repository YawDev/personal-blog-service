using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalBlog.Core.Dtos.ResponseDtos;
using PersonalBlog.Core.Interfaces.Business;

namespace PersonalBlog.Api.Controllers
{
    [ApiController]
    [Route("api")]
    public class BlogController(IBlogService blogService, IMapper mapper) : ControllerBase
    {
        private readonly IBlogService _blogService = blogService;
        private readonly IMapper _mapper = mapper;

        [AllowAnonymous]
        [HttpGet("/blogs")]
        public async Task<IActionResult> GetAllBlogs()
        {
            var blogs = await _blogService.GetAllPostsAsync();
            var response = _mapper.Map<GetAllBlogsResponseDTO>(blogs);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet("/blogs/{id}")]
        public async Task<IActionResult> GetBlogById(Guid id)
        {
            var blog = await _blogService.GetPostByIdAsync(id);
            if (blog == null)
            {
                return NotFound();
            }
            var response = _mapper.Map<GetBlogByIdResponseDTO>(blog);
            return Ok(response);
        }
    }
}