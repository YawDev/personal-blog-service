using PersonalBlog.Core.Dtos;
using PersonalBlog.Core.Dtos.RequestDtos;
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
        private readonly IDraftRepository _draftRepository;
        private readonly IUserRepository _userRepository;

        public BlogService(IBlogRepository blogRepository, IDraftRepository draftRepository, IUserRepository userRepository)
        {
            _blogRepository = blogRepository;
            _draftRepository = draftRepository;
            _userRepository = userRepository;
        }

        public async Task<SaveDraftResponseDTO> CreateDraftAsync(Guid userId, SaveDraftDTO draftDto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)           
            {
                throw new Exception("User not found");
            }

           var (draftId, result) = await _draftRepository.CreateAsync(new Draft
           {
               Id = Guid.NewGuid(),
               Userid = user.Id,
               Title = draftDto.Title,
               Content = draftDto.Content,
               Preview = draftDto.Preview,
               Createdon = DateTime.UtcNow,
               Lastmodifieddate = DateTime.UtcNow,
           });

           return new SaveDraftResponseDTO
           {
               IsSaved = result > 0,
               DraftGuid = draftId
           };
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

        public async Task<DeleteDraftResponseDTO> DeleteDraftAsync(Guid draftId, Guid userId)
        {
            var existing = await _draftRepository.GetByIdAsync(draftId);
            if (existing == null)
                return new DeleteDraftResponseDTO { IsDeleted = false, DraftGuid = draftId };

            if (existing.Userid != userId)
                throw new Exceptions.UnauthorizedException("You are not authorized to delete this draft.");

            var deleted = await _draftRepository.DeleteAsync(draftId);
            return new DeleteDraftResponseDTO { IsDeleted = deleted, DraftGuid = draftId };
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

        public async Task<GetAllDraftsByUserResponseDTO> GetAllDraftsByUserAsync(Guid userId)
        {
            var drafts = await _draftRepository.GetByUserIdAsync(userId);
            if(drafts == null || drafts.Count == 0)
            {
                return new GetAllDraftsByUserResponseDTO { UnfinishedDrafts = new List<DraftResponseDTO>() };
            }

            return new GetAllDraftsByUserResponseDTO
            {
                UnfinishedDrafts = drafts.Select(draft => new DraftResponseDTO
                {
                    Id = draft.Id,
                    Title = draft.Title,
                    Content = draft.Content,
                    Preview = draft.Preview,
                    CreatedOn = draft.Createdon,
                    UserId = draft.Userid
                }).ToList()
            };
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

        public async Task<GetDraftByIdResponseDTO> GetDraftByIdAsync(Guid draftId, Guid userId)
        {

           var draft = await _draftRepository.GetByIdAsync(draftId);
           if(draft == null)
           {
               throw new Exception("Draft not found");
           }

           if(draft.User.IdentityUserId != userId)
           {
               throw new Exceptions.UnauthorizedException("You are not authorized to view this draft.");
           }

              return new GetDraftByIdResponseDTO
              {
                Draft = new DraftResponseDTO
                {
                     Id = draft.Id,
                     Title = draft.Title,
                     Content = draft.Content,
                     Preview = draft.Preview,
                     CreatedOn = draft.Createdon,
                     UserId = draft.User.IdentityUserId
                }
              };

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

        public async Task<SaveBlogResponseDTO> PublishPostAsync(PostDTO postDto, Guid userId, Guid draftId)
        {
            var result = await _blogRepository.PublishBlogPost(draftId, userId, postDto);
            return new SaveBlogResponseDTO { IsSaved = result, PostGuid = postDto.Id };
        }

        public async Task<SaveDraftResponseDTO> UpdateDraftAsync(Guid userId, Guid draftId, SaveDraftDTO draftDto)
        {
            var result = await _draftRepository.UpdateAsync(draftId, userId, draftDto);
            return new SaveDraftResponseDTO { IsSaved = result > 0, DraftGuid = draftId };
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