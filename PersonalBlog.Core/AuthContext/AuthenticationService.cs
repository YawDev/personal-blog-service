using PersonalBlog.Core.Dtos;
using PersonalBlog.Core.Exceptions;
using PersonalBlog.Core.Interfaces;
using PersonalBlog.Models.DatabaseModels;
using PersonalBlog.Models.Dtos;

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
            catch (Exception e)
            {
                throw;
            }
            return newUser;
        }

        public async Task<(ApplicationUser, string)> AuthenticateUser(AuthenticateIdentityDTO user)
        {
            var (authenticatedUser, isSuccess) = await _userIdentityService.ValidateUserCredentialsAsync(user.UserName, user.Password);
            if (!isSuccess) //throw new FailedAuthenticationException("Invalid user credentials.");
                return (null, null);

            var accessToken = _tokenService.GenerateAccessToken(authenticatedUser);
            return (authenticatedUser, accessToken);            
        }

        public async Task<BlogUserDTO?> GetUserByIdAsync(Guid userId)
        {
            var user = await _userIdentityService.GetUserByIdAsync(userId);
            return user;
        }
    }
}