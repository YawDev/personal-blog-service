using PersonalBlog.Core.Dtos;
using PersonalBlog.Core.Dtos.ResponseDtos;
using PersonalBlog.Models.Dtos;

namespace PersonalBlog.Core.Interfaces.Business
{
    public interface IBlogService
    {
        public Task<SaveBlogResponseDTO> CreatePostAsync(CreateBlogDTO createBlogDTO, Guid userGuid);
        public Task<DeleteBlogResponseDTO> DeletePostAsync(Guid postId);
        public Task<GetBlogByIdResponseDTO?> GetPostByIdAsync(Guid postId);
        public Task<GetAllBlogsResponseDTO> GetAllPostsAsync();
        public Task<SaveBlogResponseDTO> UpdatePostAsync(PostDTO postDto);
        public Task<DeleteBlogResponseDTO> DeleteDraftAsync(Guid draftId);
        public Task<GetAllDraftsByUserResponseDTO> GetAllDraftsByUserAsync(Guid userId);
        public Task<GetDraftByIdResponseDTO> GetDraftByIdAsync(Guid draftId);
        public Task<SaveDraftResponseDTO> CreateDraftAsync(DraftDTO draftDto);
        public Task<SaveDraftResponseDTO> UpdateDraftAsync(DraftDTO draftDto);
    }
}