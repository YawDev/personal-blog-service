using PersonalBlog.Models.DatabaseModels;

namespace PersonalBlog.Core.Interfaces
{
    public interface ITokenService
    {
        public string GenerateAccessToken(ApplicationUser user);
        public string GenerateRefreshToken();
        public Task<int> SaveRefreshTokenAsync(Guid userId, string refreshTokenString);
        Task<RefreshToken?> GetAndValidateRefreshToken(string refreshTokenString);
        Task<bool> RevokeRefreshToken(RefreshToken refreshToken);
    }
}