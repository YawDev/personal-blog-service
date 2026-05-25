using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalBlog.Api.Contracts.Request;
using PersonalBlog.Core.Dtos.RequestDtos;
using PersonalBlog.Core.Interfaces.Business;
using PersonalBlog.Models.Dtos;

namespace PersonalBlog.Api.Controllers
{
    [ApiController]
    [Route("api")]
    public class DraftController(IBlogService blogService, IMapper mapper) : ControllerBase
    {
        private readonly IBlogService _blogService = blogService;
        private readonly IMapper _mapper = mapper;

        [Authorize]
        [HttpPost("/drafts/users/{userId}/create")]
        public async Task<IActionResult> CreateDraft(Guid userId, [FromBody] SaveDraftRequest createDraftRequest)
        {
            var draftDto = _mapper.Map<SaveDraftDTO>(createDraftRequest);
            var result = await _blogService.CreateDraftAsync(userId, draftDto);

            if (!result.IsSaved)
                return BadRequest("Failed to create draft");

            return Ok(result);
        }

        [Authorize]
        [HttpPut("/drafts/{draftId}/users/{userId}/edit")]
        public async Task<IActionResult> UpdateDraft(Guid userId, Guid draftId, [FromBody] SaveDraftRequest updateDraftRequest)
        {
            var draftDto = _mapper.Map<SaveDraftDTO>(updateDraftRequest);
            var result = await _blogService.UpdateDraftAsync(userId, draftId, draftDto);

            if (!result.IsSaved)
                return BadRequest("Failed to update draft");

            return Ok(result);
        }

        [Authorize]
        [HttpGet("/drafts/users/{userId}")]
        public async Task<IActionResult> GetDraftsForUser(Guid userId)
        {
            var result = await _blogService.GetAllDraftsByUserAsync(userId);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("/drafts/{draftId}/users/{userId}")]
        public async Task<IActionResult> GetDraftById(Guid userId, Guid draftId)
        {
            var result = await _blogService.GetDraftByIdAsync(draftId, userId);
            return Ok(result);
        }

        [Authorize]
        [HttpDelete("/drafts/{draftId}/users/{userId}/delete")]
        public async Task<IActionResult> DeleteDraft(Guid userId, Guid draftId)
        {
            var result = await _blogService.DeleteDraftAsync(draftId, userId);

            if (!result.IsDeleted)
                return NotFound();

            return Ok(result);
        }

        [Authorize]
        [HttpPost("/drafts/{draftId}/users/{userId}/publish")]
        public async Task<IActionResult> PublishDraft(Guid userId, Guid draftId, [FromBody] CreateBlogRequest publishBlogRequest)
        {
            var result = await _blogService.PublishPostAsync(new PostDTO { Title = publishBlogRequest.Title, Content = publishBlogRequest.Content }, userId, draftId);

            if (!result.IsSaved)
                return BadRequest("Failed to publish blog post");

            return Ok(result);
        }

    }
}