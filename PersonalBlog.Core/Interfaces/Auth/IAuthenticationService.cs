using PersonalBlog.Core.Dtos;
using PersonalBlog.Models.DatabaseModels;
using PersonalBlog.Models.Dtos;

namespace PersonalBlog.Core.Interfaces
{
    public interface IAuthenticationService
    {
        Task<ApplicationUser> CreateUserAndIdentity(CreateIdentityDTO user);
        Task<(ApplicationUser, string)> AuthenticateUser(AuthenticateIdentityDTO user);
        Task<BlogUserDTO?> GetUserByIdAsync(Guid userId);
    }
}