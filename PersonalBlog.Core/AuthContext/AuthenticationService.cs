using PersonalBlog.Core.Dtos;
using PersonalBlog.Core.Dtos.RequestDtos;
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
    }
}