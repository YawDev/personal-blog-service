using PersonalBlog.Core.Dtos;
using PersonalBlog.Models.DatabaseModels;

namespace PersonalBlog.Core.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<int> CreateAsync(BlogUser user);
        Task<int> CreateIdentityUserAsync(ApplicationUser user);
        Task<bool> DeleteAsync(Guid userId);
        Task<bool> ExistsAsync(Guid userId);
        Task<IdentityUserDTO?> GetByEmailAsync(string email);
        Task<BlogUserDTO?> GetByIdAsync(Guid userId);
        Task<ApplicationUser?> GetByUserNameAsync(string userName);
        Task<IdentityUserDTO?> GetIdentityUserInfoAsync(Guid id);
        Task<ApplicationUser> UpdateAsync(ApplicationUser user);
        Task<bool> ValidateCredentialsAsync(string userName, string passwordHash);
    }
}