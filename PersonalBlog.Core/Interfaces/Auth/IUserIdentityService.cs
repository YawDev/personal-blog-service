using PersonalBlog.Core.Dtos;
using PersonalBlog.Models.DatabaseModels;
using PersonalBlog.Models.Dtos;

namespace PersonalBlog.Core.Interfaces
{
    public interface IUserIdentityService
    {
        Task<ApplicationUser> CreateUserAndIdentityAsync(CreateIdentityDTO user);
        Task<(ApplicationUser, bool)> ValidateUserCredentialsAsync(string userName, string password);
        Task<BlogUserDTO?> GetUserByIdAsync(Guid userId);
        Task<IdentityUserDTO?> GetIdentityUserInfo(Guid userId);
        Task<bool> UpdateUserAsync(Guid userId, EditAccountDTO editAccountRequest);
    }
}