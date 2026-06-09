using PersonalBlog.Core.Dtos;
using PersonalBlog.Core.Dtos.RequestDtos;
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
        Task<bool> UpdateIdentityUserAsync(Guid identityUserId, EditAccountDTO editAccountRequest);
        Task<bool> UpdateBlogUserAsync(Guid identityUserId, EditAccountDTO editAccount);
        Task<bool> ValidateCredentialsAsync(string userName, string passwordHash);
        Task<ApplicationUser?> GetApplicationUserAsync(Guid id);
    }
}