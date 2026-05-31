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
        Task<(ApplicationUser user, string newAccessToken, string newRefreshToken)> RefreshUserSession(string oldRefreshToken);
        Task<bool> RevokeRefreshToken(RefreshToken refreshToken);
        Task<RefreshToken> FindAndValidateRefreshToken(string refreshToken);
        Task RevokeUserSession(string refreshToken);
    }
}