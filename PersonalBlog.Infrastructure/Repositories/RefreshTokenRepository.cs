

using Microsoft.EntityFrameworkCore;
using PersonalBlog.Core.Interfaces.Repositories;
using PersonalBlog.Models.DatabaseModels;

namespace PersonalBlog.Infrastructure.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly PersonalBlogDbContext _context;

        public RefreshTokenRepository(PersonalBlogDbContext context)
        {
            _context = context;
        }

        public async Task<int> CreateAsync(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Add(refreshToken);
            return await _context.SaveChangesAsync();            
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == token);
        }

        public async Task<bool> RevokeAsync(string token)
        {
            var refreshToken = await GetByTokenAsync(token);
            if (refreshToken == null || refreshToken.IsRevoked || refreshToken.IsUsed || refreshToken.IsExpired)
                return false;

            refreshToken.IsRevoked = true;
            _context.RefreshTokens.Update(refreshToken);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}