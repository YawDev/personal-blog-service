using AutoMapper;
using PersonalBlog.Core.Interfaces.Repositories;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using PersonalBlog.Core.Dtos;
using PersonalBlog.Models.DatabaseModels;
using PersonalBlog.Models.Dtos;


namespace PersonalBlog.Infrastructure.Repositories
{
    public class BlogRepository(PersonalBlogDbContext context, IMapper mapper) : IBlogRepository
    {
        private readonly PersonalBlogDbContext _context = context;
        private readonly IMapper _mapper = mapper;

        public async Task<int> CreateAsync(Post post)
        {
            _context.Posts.Add(post);
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(Guid postId)
        {
            var post = await _context.Posts.FindAsync(postId);
            if (post == null) return false;
            _context.Posts.Remove(post);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ExistsAsync(Guid postId)
        {
            var existingPost = await _context.Posts.FindAsync(postId);
            return existingPost != null;
        }

        public async Task<List<PostDTO>> GetAllAsync()
        {
            var posts = await _context.Posts
                .Include(p => p.User)
                .ProjectTo<PostDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();
            return posts;
        }

        public async Task<PostDTO?> GetByIdAsync(Guid postId)
        {
            var post = await _context.Posts
                .Include(p => p.User)
                .ProjectTo<PostDTO>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(p => p.Id == postId);
            return post;
        }

        public async Task<int> UpdateAsync(Post post)
        {
            _context.Posts.Update(post);
            return await _context.SaveChangesAsync();
        }
    }
}