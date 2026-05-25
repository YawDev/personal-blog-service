using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using PersonalBlog.Core.Dtos;
using PersonalBlog.Core.Dtos.RequestDtos;
using PersonalBlog.Core.Interfaces.Repositories;
using PersonalBlog.Models.DatabaseModels;

namespace PersonalBlog.Infrastructure.Repositories
{  
    public class UserRepository(PersonalBlogDbContext context, IMapper mapper) : IUserRepository
    {
        private readonly PersonalBlogDbContext _context = context;
        private readonly IMapper _mapper = mapper;

        public async Task<int> CreateAsync(BlogUser user)
        {
            _context.BlogUsers.Add(user);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> CreateIdentityUserAsync(ApplicationUser user)
        {
            _context.ApplicationUsers.Add(user);
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(Guid userId)
        {
            var user = _context.ApplicationUsers.Where(u => u.Id == userId);
            _context.Remove(user);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ExistsAsync(Guid userId)
        {
            var existingUser = await _context.ApplicationUsers.FindAsync(userId);
            return existingUser != null;
        }

        public async Task<IdentityUserDTO?> GetByEmailAsync(string email)
        {
            var existingUser = await _context.ApplicationUsers
                .Where(u => u.Email == email)
                .ProjectTo<IdentityUserDTO>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
            return existingUser;
        }

        public async Task<BlogUserDTO?> GetByIdAsync(Guid identityUserId)
        {
            var existingUser = await _context.BlogUsers
                .Include(x => x.Posts)
                .Include(x => x.Drafts)
                .ProjectTo<BlogUserDTO>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(u => u.IdentityUserId == identityUserId);
            return existingUser;
        }

        public async Task<ApplicationUser?> GetByUserNameAsync(string userName)
        {
            var existingUser = await _context.ApplicationUsers
                .FirstOrDefaultAsync(u => u.UserName == userName);
            return existingUser;
        }

        public async Task<IdentityUserDTO?> GetIdentityUserInfoAsync(Guid id)
        {
            var existingUser = await _context.ApplicationUsers
                .ProjectTo<IdentityUserDTO>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(iu => iu.Id == id);
            return existingUser;
        }

        public async Task<bool> UpdateBlogUserAsync(Guid identityUserId, EditAccountDTO editAccountRequest)
        {
            // Also update linked FK BlogUser table with same changes to avoid data inconsistency
            var blogUser = await _context.BlogUsers.FirstOrDefaultAsync(u => u.IdentityUserId == identityUserId);
            
            blogUser.Lastmodifieddate = DateTime.UtcNow;
            blogUser.Username = editAccountRequest.UserName;
            blogUser.Email = editAccountRequest.Email;

            _context.BlogUsers.Update(blogUser);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateIdentityUserAsync(Guid identityUserId, EditAccountDTO editAccountRequest)
        {
            var identityUser = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == identityUserId);
            if (identityUser == null) return false;

            identityUser.UserName = editAccountRequest.UserName;
            identityUser.Email = editAccountRequest.Email;
            identityUser.NormalizedUserName = editAccountRequest.UserName.ToUpper();
            identityUser.NormalizedEmail = editAccountRequest.Email.ToUpper();
            // identityUser.FirstName = editAccountRequest.FirstName;
            // identityUser.LastName = editAccountRequest.LastName;

            _context.ApplicationUsers.Update(identityUser);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ValidateCredentialsAsync(string userName, string passwordHash)
        {
            var existingUser = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.UserName == userName && u.PasswordHash == passwordHash);
            return existingUser != null;
        }
    }
}