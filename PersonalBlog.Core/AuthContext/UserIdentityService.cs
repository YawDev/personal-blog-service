using Microsoft.AspNetCore.Identity;
using PersonalBlog.Core.Dtos;
using PersonalBlog.Core.Dtos.RequestDtos;
using PersonalBlog.Core.Exceptions;
using PersonalBlog.Core.Interfaces;
using PersonalBlog.Core.Interfaces.Repositories;
using PersonalBlog.Models.DatabaseModels;

namespace PersonalBlog.Core.AuthContext
{
    public class UserIdentityService(IUserRepository userRepository, IPasswordHasher<ApplicationUser> passwordHasher) : IUserIdentityService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher = passwordHasher;

        public async Task<ApplicationUser> CreateUserAndIdentityAsync(CreateIdentityDTO user)
        {
            var existingUser = await _userRepository.GetByUserNameAsync(user.UserName);

            if (existingUser != null) throw new BadRequestException("User already exists");

            var newIdentity = new ApplicationUser
            {
                UserName = user.UserName,
                Email = user.Email,
                NormalizedEmail = user.Email.ToUpper(),
                NormalizedUserName = user.UserName.ToUpper(),
                SecurityStamp = Guid.NewGuid().ToString() // Generate security stamp
            };

            newIdentity.PasswordHash = _passwordHasher.HashPassword(newIdentity, user.Password);

            var result = await _userRepository.CreateIdentityUserAsync(newIdentity);
            if(result > 0)
            {
                var newUser = new BlogUser
                {
                    Id = Guid.NewGuid(),
                    IdentityUserId = newIdentity.Id,
                    Username = user.UserName,
                    Email = user.Email,
                    Role = Models.Enums.UserRole.User,
                    Createddate = DateTime.UtcNow,
                    Lastmodifieddate = DateTime.UtcNow
                };
                await _userRepository.CreateAsync(newUser);
            }

            return newIdentity;
        }

        public async Task<IdentityUserDTO?> GetIdentityUserInfo(Guid userId)
        {
            return await _userRepository.GetIdentityUserInfoAsync(userId);
        }

        public async Task<IdentityUserDTO?> GetUserByEmailAsync(string email)
        {
            return await _userRepository.GetByEmailAsync(email);
        }
        
        public async Task<BlogUserDTO?> GetUserByIdAsync(Guid identityUserId)
        {
            var user = await _userRepository.GetByIdAsync(identityUserId) ?? throw new BadRequestException("User not found");
            return user;
        }

        public async Task<ApplicationUser?> GetUserByUserNameAsync(string userName)
        {
            return await _userRepository.GetByUserNameAsync(userName);
        }

        public async Task<ApplicationUser?> GetApplicationUserAsync(Guid id)
        {
            return await _userRepository.GetApplicationUserAsync(id);
        }

        public async Task<bool> UpdateUserAsync(Guid userId, EditAccountDTO editAccountRequest)
        {
            // Update the user's account details in the identity system
            var updateIdentityResult = await _userRepository.UpdateBlogUserAsync(userId, editAccountRequest);

            // Update FK BlogUser table with same changes to avoid data inconsistency   
            var updateBlogUserResult = await _userRepository.UpdateIdentityUserAsync(userId, editAccountRequest);

            return updateIdentityResult && updateBlogUserResult;
        }       
        
        public async Task<(ApplicationUser?,bool)> ValidateUserCredentialsAsync(string userName, string password)
        {
            var user = await _userRepository.GetByUserNameAsync(userName);
            
            if (user == null) return (null, false);

            // Verify the password using PasswordHasher
            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
            
            return (user, result == PasswordVerificationResult.Success);
        }
    }
}