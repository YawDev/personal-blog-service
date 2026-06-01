using PersonalBlog.Core.Dtos;
using PersonalBlog.Core.Dtos.RequestDtos;
using PersonalBlog.Core.Exceptions;
using PersonalBlog.Core.Interfaces;
using PersonalBlog.Models.DatabaseModels;

namespace PersonalBlog.Core.AuthContext
{
    public class AuthenticationService(IUserIdentityService userIdentityService, ITokenService tokenService) : IAuthenticationService
    {
        private readonly IUserIdentityService _userIdentityService = userIdentityService;
        private readonly ITokenService _tokenService = tokenService;
        public async Task<ApplicationUser> CreateUserAndIdentity(CreateIdentityDTO user)
        {
            ApplicationUser? newUser = null;
            try
            {
                newUser = await _userIdentityService.CreateUserAndIdentityAsync(user);
                if (newUser == null)
                {
                    throw new Exception("Failed to create user and identity");
                }
            }
            catch (Exception)
            {
                throw;
            }
            return newUser;
        }

        public async Task<(ApplicationUser?, string? accessToken, string? refreshToken)> AuthenticateUser(AuthenticateIdentityDTO user)
        {
            var (authenticatedUser, isSuccess) = await _userIdentityService.ValidateUserCredentialsAsync(user.UserName, user.Password);
            if (!isSuccess) //throw new FailedAuthenticationException("Invalid user credentials.");
                return (null, null, null);

            var accessToken = _tokenService.GenerateAccessToken(authenticatedUser);
            var refreshToken = _tokenService.GenerateRefreshToken();
            // Save the refresh token to the database or any persistent storage associated with the user for later validation
            await _tokenService.SaveRefreshTokenAsync(authenticatedUser.Id, refreshToken);

            return (authenticatedUser, accessToken, refreshToken);            
        }

        public async Task<BlogUserDTO?> GetUserByIdAsync(Guid userId)
        {
            var user = await _userIdentityService.GetUserByIdAsync(userId);
            return user;
        }

        public async Task<IdentityUserDTO?> GetIdentityUserAsync(Guid identityUserId)
        {
            var user = await _userIdentityService.GetIdentityUserInfo(identityUserId);
            return user;
        }

        public async Task<(ApplicationUser user, string newAccessToken, string newRefreshToken)> RefreshUserSession(string oldRefreshToken)
        {
            var refreshToken = await FindAndValidateRefreshToken(oldRefreshToken);
            var identityUser = refreshToken.IdentityUser ?? throw new UnauthorizedException("Not able to find user tied to existing token");

            // mark old as isUsed = true
            await RevokeRefreshToken(refreshToken);

            //generate new access token + new refresh token
            var (newAccessToken, newRefreshToken) = await GenerateNewTokens(identityUser.Id);

            return (identityUser, newAccessToken, newRefreshToken);
        }

        public async Task<RefreshToken> FindAndValidateRefreshToken(string refreshToken)
        {
            var refreshTokenEntity = await _tokenService.GetAndValidateRefreshToken(refreshToken);
            if (refreshTokenEntity == null || !refreshTokenEntity.IsActive)
                throw new UnauthorizedException("Invalid or expired refresh token.");

            return refreshTokenEntity;
        }

        public async Task<bool> RevokeRefreshToken(RefreshToken refreshToken)
        {
            return await _tokenService.RevokeRefreshToken(refreshToken);
        }

        public async Task RevokeUserSession(string refreshToken)
        {
            var token = await _tokenService.GetAndValidateRefreshToken(refreshToken);
            if (token == null) return;
            await _tokenService.RevokeAllForUserAsync(token.IdentityUserId);
        }

        private async Task<(string accessToken, string refreshToken)> GenerateNewTokens(Guid identityUserId)
        {

            var applicationUser = await _userIdentityService.GetApplicationUserAsync(identityUserId);

            if (applicationUser is null)
                throw new UnauthorizedException("User not found for refresh token.");

            var accessToken = _tokenService.GenerateAccessToken(applicationUser);
            var refreshToken = _tokenService.GenerateRefreshToken();
            // Save the refresh token to the database or any persistent storage associated with the user for later validation
            await _tokenService.SaveRefreshTokenAsync(applicationUser.Id, refreshToken);

            return (accessToken, refreshToken);            
        }
    }
}