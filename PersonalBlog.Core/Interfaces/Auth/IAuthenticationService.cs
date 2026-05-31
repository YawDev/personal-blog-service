using PersonalBlog.Core.Dtos;
using PersonalBlog.Core.Dtos.RequestDtos;
using PersonalBlog.Models.DatabaseModels;

namespace PersonalBlog.Core.Interfaces
{
    public interface IAuthenticationService
    {
        Task<ApplicationUser> CreateUserAndIdentity(CreateIdentityDTO user);
        Task<(ApplicationUser?, string? accessToken, string? refreshToken)> AuthenticateUser(AuthenticateIdentityDTO user);
        Task<BlogUserDTO?> GetUserByIdAsync(Guid userId);
        Task<IdentityUserDTO?> GetIdentityUserAsync(Guid identityUserId);
        Task<RefreshToken?> GetAndValidateRefreshToken(string refreshToken);
    }
}