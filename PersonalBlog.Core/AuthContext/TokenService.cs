using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PersonalBlog.Core.Interfaces;
using PersonalBlog.Core.Interfaces.Repositories;
using PersonalBlog.Models.DatabaseModels;

namespace PersonalBlog.Core.AuthContext
{
    public class TokenService(IConfiguration configuration, IRefreshTokenRepository refreshTokenRepository) : ITokenService
    {
        private readonly string _secretKey = configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured");
        private readonly string _issuer = configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT Issuer not configured");
        private readonly string _audience = configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT Audience not configured");
        private readonly int _accessTokenExpirationMinutes = int.TryParse(configuration["Jwt:AccessTokenExpirationMinutes"], out var minutes) ? minutes : 30;
        private readonly int _refreshTokenExpirationDays = int.TryParse(configuration["Jwt:RefreshTokenExpirationDays"], out var days) ? days : 7;

        private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;

        public string GenerateAccessToken(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.UserName ?? "" ),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(ClaimTypes.NameIdentifier, user.Id.ToString())};

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_accessTokenExpirationMinutes), // Token expires in 30 minutes
                signingCredentials: creds );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);  
            return Convert.ToBase64String(randomBytes);
        }

        public async Task<int> SaveRefreshTokenAsync(Guid userId, string refreshToken)
        {
           var refreshTokenEntity = new RefreshToken
           {
                Id = Guid.NewGuid(),
                IdentityUserId = userId,
                Token = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(_refreshTokenExpirationDays) // Refresh token expires in 7 days
           };

           var result = await _refreshTokenRepository.CreateAsync(refreshTokenEntity);

           if(result <= 0)
           {
               throw new Exception("Failed to save refresh token");
           }

           return result;
        }
    }
}