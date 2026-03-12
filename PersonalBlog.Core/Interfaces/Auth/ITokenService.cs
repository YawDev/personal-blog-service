using PersonalBlog.Models.DatabaseModels;

namespace PersonalBlog.Core.Interfaces
{
    public interface ITokenService
    {
         public string GenerateAccessToken(ApplicationUser user);
        public string GenerateRefreshToken();
    }
}