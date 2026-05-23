using PersonalBlog.Models.DatabaseModels;
using PersonalBlog.Models.Dtos;

namespace PersonalBlog.Core.Interfaces.Repositories
{
    public interface IBlogRepository
    {
        Task<int> CreateAsync(Post post);
        Task<bool> DeleteAsync(Guid postId);
        Task<bool> ExistsAsync(Guid postId);
        Task<PostDTO?> GetByIdAsync(Guid postId);
        Task<List<PostDTO>> GetAllAsync();
        Task<int> UpdateAsync(Guid postId, PostDTO post);
        Task<bool> PublishBlogPost(Guid draftId, Guid userId, PostDTO post);

    }
}