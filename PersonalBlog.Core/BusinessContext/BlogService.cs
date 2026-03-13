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

        public Task<DeleteBlogResponseDTO> DeletePostAsync(Guid postId)
        {
            throw new NotImplementedException();
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
                    UserId = blog.Userid,
                }).ToList()
            };
        }

        public Task<GetDraftByIdResponseDTO> GetDraftByIdAsync(Guid draftId)
        {
            throw new NotImplementedException();
        }

        public Task<GetBlogByIdResponseDTO?> GetPostByIdAsync(Guid postId)
        {
            throw new NotImplementedException();
        }

        public Task<SaveDraftResponseDTO> UpdateDraftAsync(DraftDTO draftDto)
        {
            throw new NotImplementedException();
        }

        public Task<SaveBlogResponseDTO> UpdatePostAsync(PostDTO postDto)
        {
            throw new NotImplementedException();
        }
    }
}