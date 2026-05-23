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

        public async Task<bool> PublishBlogPost(Guid draftId, Guid userId, PostDTO post)
        {
            var draft = await _context.Drafts.FirstOrDefaultAsync(d => d.Id == draftId);
            if (draft == null) return false;

            if (draft.User.IdentityUserId != userId)
                throw new UnauthorizedAccessException("You do not have permission to publish this draft.");

            var newPost = new Post
            {
                Id = Guid.NewGuid(),
                Userid = userId,
                Title = post.Title,
                Content = post.Content,
                Preview = post.Preview,
                Createddate = DateTime.UtcNow,
                Lastmodifieddate = DateTime.UtcNow
            };

            _context.Posts.Add(newPost);
            _context.Drafts.Remove(draft);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<int> UpdateAsync(Guid postId, PostDTO post)
        {
            var existingPost = await _context.Posts.FindAsync(postId);
            if (existingPost == null) return 0;

            existingPost.Title = post.Title;
            existingPost.Content = post.Content;
            existingPost.Preview = post.Preview;
            existingPost.Lastmodifieddate = DateTime.UtcNow;

            _context.Posts.Update(existingPost);
            return await _context.SaveChangesAsync();
        }
    }
}