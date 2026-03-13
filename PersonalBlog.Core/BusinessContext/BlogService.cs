using PersonalBlog.Core.Dtos;
using PersonalBlog.Core.Dtos.ResponseDtos;
using PersonalBlog.Core.Interfaces.Business;
using PersonalBlog.Core.Interfaces.Repositories;
using PersonalBlog.Models.Dtos;

namespace PersonalBlog.Core.BusinessContext
{
    public class BlogService : IBlogService
    {
        private readonly IBlogRepository _blogRepository;
        public BlogService(IBlogRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }

        public Task<SaveDraftResponseDTO> CreateDraftAsync(DraftDTO draftDto)
        {
            throw new NotImplementedException();
        }

        public Task<SaveBlogResponseDTO> CreatePostAsync(PostDTO postDto)
        {
            throw new NotImplementedException();
        }

        public Task<DeleteBlogResponseDTO> DeleteDraftAsync(Guid draftId)
        {
            throw new NotImplementedException();
        }

        public Task<DeleteBlogResponseDTO> DeletePostAsync(Guid postId)
        {
            throw new NotImplementedException();
        }

        public Task<GetAllDraftsByUserResponseDTO> GetAllDraftsByUserAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public async Task<GetAllBlogsResponseDTO> GetAllPostsAsync()
        {
            var blogs = await _blogRepository.GetAllAsync();
            // You can map the blogs to your response DTO here
            return new GetAllBlogsResponseDTO
            {
                Blogs = blogs
            };
        }

        public Task<GetDraftByIdResponseDTO> GetDraftByIdAsync(Guid draftId)
        {
            throw new NotImplementedException();
        }

        public Task<GetBlogByIdResponseDTO?> GetPostByIdAsync(Guid postId)
        {
            throw new NotImplementedException();
        }

        public Task<SaveDraftResponseDTO> UpdateDraftAsync(DraftDTO draftDto)
        {
            throw new NotImplementedException();
        }

        public Task<SaveBlogResponseDTO> UpdatePostAsync(PostDTO postDto)
        {
            throw new NotImplementedException();
        }
    }
}