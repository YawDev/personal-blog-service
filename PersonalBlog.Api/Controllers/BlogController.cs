using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalBlog.Api.ActionFilters;
using PersonalBlog.Api.Contracts.Request;
using PersonalBlog.Core.Dtos.ResponseDtos;
using PersonalBlog.Core.Interfaces.Business;
using PersonalBlog.Models.Dtos;

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

        [IdentityFilter]
        [HttpPost("/blogs/create/{id}")]
        public async Task<IActionResult> CreateBlog(Guid id, [FromBody] CreateBlogRequest createBlogRequest)
        {
            var createBlogDTO = _mapper.Map<CreateBlogDTO>(createBlogRequest);
            
            var response = await _blogService.CreatePostAsync(createBlogDTO, id);
            return Ok(response);            
        }
    }
}