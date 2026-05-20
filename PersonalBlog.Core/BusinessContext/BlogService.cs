using PersonalBlog.Core.Dtos;
using PersonalBlog.Core.Dtos.ResponseDtos;
using PersonalBlog.Core.Interfaces.Business;
using PersonalBlog.Core.Interfaces.Repositories;
using PersonalBlog.Models.DatabaseModels;
using PersonalBlog.Models.Dtos;

namespace PersonalBlog.Core.BusinessContext
{
    public class BlogService : IBlogService
    {
        private readonly IBlogRepository _blogRepository;
        private readonly IUserRepository _userRepository;

        public BlogService(IBlogRepository blogRepository, IUserRepository userRepository)
        {
            _blogRepository = blogRepository;
            _userRepository = userRepository;
        }

        public Task<SaveDraftResponseDTO> CreateDraftAsync(DraftDTO draftDto)
        {
            throw new NotImplementedException();
        }

        public async Task<SaveBlogResponseDTO> CreatePostAsync(CreateBlogDTO postDto, Guid userGuid)
        {
            var blogUser = await _userRepository.GetByIdAsync(userGuid);
            if (blogUser == null)
            {
                throw new Exception("User not found");
            }
            
            var newPost = new Post
            {
                Id = Guid.NewGuid(),
                Userid = blogUser.Id,
                Title = postDto.Title,
                Content = postDto.Content,
                Preview = postDto.Preview,
                Createddate = DateTime.UtcNow,
                Lastmodifieddate = DateTime.UtcNow
            };
            var result = await _blogRepository.CreateAsync(newPost);
            
            return new SaveBlogResponseDTO
            {
                IsSaved = result > 0,
                 PostGuid = newPost.Id
            };
        }

        public Task<DeleteBlogResponseDTO> DeleteDraftAsync(Guid draftId)
        {
            throw new NotImplementedException();
        }

        public async Task<DeleteBlogResponseDTO> DeletePostAsync(Guid postId, Guid userId)
        {
            var existing = await _blogRepository.GetByIdAsync(postId);
            if (existing == null)
                return new DeleteBlogResponseDTO { IsDeleted = false, PostGuid = postId };

            if (existing.User.IdentityUserId != userId)
                throw new Exceptions.UnauthorizedException("You are not authorized to delete this post.");

            var deleted = await _blogRepository.DeleteAsync(postId);
            return new DeleteBlogResponseDTO { IsDeleted = deleted, PostGuid = postId };
        }

        public Task<GetAllDraftsByUserResponseDTO> GetAllDraftsByUserAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public async Task<GetAllBlogsResponseDTO> GetAllPostsAsync()
        {
            var blogs = await _blogRepository.GetAllAsync();
            // You can map the blogs to your response DTO here
            return new GetAllBlogsResponseDTO
            {
                Blogs = blogs.Select(blog => new BlogResponseDTO
                {
                    Id = blog.Id,
                    Title = blog.Title,
                    Content = blog.Content,
                    Preview = blog.Preview,
                    DatePosted = blog.Dateposted,
                    UserId = blog.User.IdentityUserId,
                }).ToList()
            };
        }

        public Task<GetDraftByIdResponseDTO> GetDraftByIdAsync(Guid draftId)
        {
            throw new NotImplementedException();
        }

        public async Task<GetBlogByIdResponseDTO?> GetPostByIdAsync(Guid postId)
        {
            var post = await _blogRepository.GetByIdAsync(postId);
            
            if(post == null)
            {
                throw new Exception("Post not found");
            }
            
            return new GetBlogByIdResponseDTO
            {
                Blog = new BlogResponseDTO
                {
                    Id = post.Id,
                    Title = post.Title,
                    Content = post.Content,
                    Preview = post.Preview,
                    DatePosted = post.Dateposted,
                    UserId = post.User.IdentityUserId
                }
            };
            
        }

        public Task<SaveDraftResponseDTO> UpdateDraftAsync(DraftDTO draftDto)
        {
            throw new NotImplementedException();
        }

        public async Task<SaveBlogResponseDTO> UpdatePostAsync(PostDTO postDto, Guid userId)
        {
            if (string.IsNullOrWhiteSpace(postDto.Title) ||
                string.IsNullOrWhiteSpace(postDto.Content) ||
                string.IsNullOrWhiteSpace(postDto.Preview))
                throw new Exceptions.BadRequestException("Title, content, and preview are required.");

            var existing = await _blogRepository.GetByIdAsync(postDto.Id);
            if (existing == null)
                return new SaveBlogResponseDTO { IsSaved = false, PostGuid = postDto.Id };

            if (existing.User.IdentityUserId != userId)
                throw new Exceptions.UnauthorizedException("You are not authorized to edit this post.");

            existing.Title = postDto.Title;
            existing.Content = postDto.Content;
            existing.Preview = postDto.Preview;
            existing.Lastmodifieddate = DateTime.UtcNow;

            var result = await _blogRepository.UpdateAsync(existing.Id, existing);
            
            return new SaveBlogResponseDTO { IsSaved = result > 0, PostGuid = existing.Id };
        }
    }
}