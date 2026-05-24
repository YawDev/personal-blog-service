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
        [HttpPut("/drafts/{draftId}/users/{id}/edit")]
        public async Task<IActionResult> UpdateDraft(Guid id, Guid draftId, [FromBody] SaveDraftRequest updateDraftRequest)
        {
            var draftDto = _mapper.Map<SaveDraftDTO>(updateDraftRequest);
            var result = await _blogService.UpdateDraftAsync(id, draftId, draftDto);

            if (!result.IsSaved)
                return BadRequest("Failed to update draft");

            return Ok(result);
        }

        [Authorize]
        [HttpGet("/drafts/users/{id}")]
        public async Task<IActionResult> GetDraftsForUser(Guid id)
        {
            var result = await _blogService.GetAllDraftsByUserAsync(id);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("/drafts/{draftId}/users/{id}")]
        public async Task<IActionResult> GetDraftById(Guid id, Guid draftId)
        {
            var result = await _blogService.GetDraftByIdAsync(draftId, id);
            return Ok(result);
        }

        [Authorize]
        [HttpDelete("/drafts/{draftId}/users/{id}/delete")]
        public async Task<IActionResult> DeleteDraft(Guid id, Guid draftId)
        {
            var result = await _blogService.DeleteDraftAsync(draftId, id);

            if (!result.IsDeleted)
                return NotFound();

            return Ok(result);
        }

        [Authorize]
        [HttpPost("/drafts/{draftId}/users/{id}/publish")]
        public async Task<IActionResult> PublishDraft(Guid id, Guid draftId, [FromBody] CreateBlogRequest publishBlogRequest)
        {
            var result = await _blogService.PublishPostAsync(new PostDTO { Title = publishBlogRequest.Title, Content = publishBlogRequest.Content }, id, draftId);

            if (!result.IsSaved)
                return BadRequest("Failed to publish blog post");

            return Ok(result);
        }

    }
}