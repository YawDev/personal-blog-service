using PersonalBlog.Core.Dtos;
using PersonalBlog.Core.Dtos.RequestDtos;
using PersonalBlog.Core.Dtos.ResponseDtos;
using PersonalBlog.Models.Dtos;

namespace PersonalBlog.Core.Interfaces.Business
{
    public interface IBlogService
    {
        public Task<SaveBlogResponseDTO> CreatePostAsync(CreateBlogDTO createBlogDTO, Guid userGuid);
        public Task<DeleteBlogResponseDTO> DeletePostAsync(Guid postId, Guid userId);
        public Task<GetBlogByIdResponseDTO?> GetPostByIdAsync(Guid postId);
        public Task<GetAllBlogsResponseDTO> GetAllPostsAsync();
        public Task<SaveBlogResponseDTO> UpdatePostAsync(PostDTO postDto, Guid userId);
        public Task<DeleteDraftResponseDTO> DeleteDraftAsync(Guid draftId, Guid userId);
        public Task<GetAllDraftsByUserResponseDTO> GetAllDraftsByUserAsync(Guid userId);
        public Task<GetDraftByIdResponseDTO> GetDraftByIdAsync(Guid draftId, Guid userId);
        public Task<SaveDraftResponseDTO> CreateDraftAsync(Guid userId, SaveDraftDTO draftDto);
        public Task<SaveDraftResponseDTO> UpdateDraftAsync(Guid userId, Guid draftId, SaveDraftDTO draftDto);
        public Task<SaveBlogResponseDTO> PublishPostAsync(PostDTO postDto, Guid userId, Guid draftId);

    }
}