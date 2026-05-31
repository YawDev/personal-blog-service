using PersonalBlog.Models.DatabaseModels;

namespace PersonalBlog.Core.Interfaces.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task<int> CreateAsync(RefreshToken refreshToken);
        Task<RefreshToken?> GetByTokenAsync(string token);
        Task<bool> RevokeAsync(string token);
    }
}