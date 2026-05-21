using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalBlog.Api.ActionFilters;
using PersonalBlog.Api.Contracts.Request;
using PersonalBlog.Api.Contracts.Response.Blogs;
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
            var response = _mapper.Map<GetAllBlogsResponse>(blogs);
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
            var response = _mapper.Map<GetBlogByIdResponse>(blog);
            return Ok(response);
        }

        [Authorize]
        [HttpPost("/blogs/create/{id}")]
        public async Task<IActionResult> CreateBlog(Guid id, [FromBody] CreateBlogRequest createBlogRequest)
        {
            var createBlogDTO = _mapper.Map<CreateBlogDTO>(createBlogRequest);

            var createdBlog = await _blogService.CreatePostAsync(createBlogDTO, id);

            var response = _mapper.Map<SaveBlogResponse>(createdBlog);

            return Ok(response);
        }

        [Authorize]
        [HttpPut("/blogs/{postId}/users/{id}")]
        public async Task<IActionResult> UpdateBlog(Guid id, Guid postId, [FromBody] UpdateBlogRequest updateBlogRequest)
        {
            updateBlogRequest.Id = postId;
            var postDto = _mapper.Map<PostDTO>(updateBlogRequest);
            var result = await _blogService.UpdatePostAsync(postDto, id);

            if (!result.IsSaved)
                return NotFound();

            return Ok(_mapper.Map<SaveBlogResponse>(result));
        }

        [Authorize]
        [HttpDelete("/blogs/{postId}/users/{id}/delete")]
        public async Task<IActionResult> DeleteBlog(Guid id, Guid postId)
        {
            var result = await _blogService.DeletePostAsync(postId, id);

            if (!result.IsDeleted)
                return NotFound();

            return Ok(_mapper.Map<DeleteBlogResponse>(result));
        }
    }
}